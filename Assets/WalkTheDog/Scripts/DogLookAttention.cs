using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogLookAttention : MonoBehaviour
{
    public DogRefs dogRefs;

    public DogAstar dogAstar => dogRefs.dogBrain.dogAstar;

    // this is where it looks with the head/eyes.
    private Transform _currentLookObject;

    private Vector3 lookTarget;
    public float lookSmoothness = 0.2f;

    public float headLookAngleLimit = 45;

    public bool lookingAtVector = false;
    public bool lookAtForward = false;
    public float factor = 3f;

    public void LookAt(Transform target)
    {
        _currentLookObject = target;
        lookingAtVector = false;
    }

    public void LookAtPosition(Vector3 targetPos)
    {
        _currentLookObject = null;
        lookingAtVector = true;
        lookTarget = targetPos;

    }

    public void LookAtDirection(Vector3 dir)
    {
        _currentLookObject = null;
        lookingAtVector = true;
        lookTarget = dogRefs.headForwardNoRotation.position + dir;
    }

    void Update()
    {

        // if we have a current look object
        if (_currentLookObject != null && !lookingAtVector)
        {
            // look at the current look object.
            lookTarget = _currentLookObject.position;
        }
        else if (lookingAtVector)
        {
            // look at the vector. lookTarget already set.
        }
        else
        if (lookAtForward)
        {
            lookTarget = dogRefs.headForwardNoRotation.position + dogRefs.headForwardNoRotation.forward;
        }
        // if we're currently moving, look at node[i+1]
        else if (dogAstar.hasDestination && dogAstar.hasPath)
        {
            var nextNextNode = dogAstar.GetNextNextNode();
            if (nextNextNode != null)
            {
                lookTarget = nextNextNode.position;
            }
        }
        else
        {
            // look at nothing
        }


        // animate head
        Update_AnimateHead();

    }

    private void Update_AnimateHead()
    {
        if ((_currentLookObject != null) || lookingAtVector)
        {
            var localLookTarget = dogRefs.headForwardNoRotation.InverseTransformPoint(this.lookTarget);
            localLookTarget.Normalize();

            var dot = Vector3.Dot(dogRefs.head.forward, dogRefs.headForwardNoRotation.forward);
            if (dot < 0.5f)
            {
                localLookTarget += Vector3.forward * (0.5f - dot) * factor;
            }

            var finalLookTarget = dogRefs.headForwardNoRotation.TransformPoint(localLookTarget);

            Debug.DrawLine(dogRefs.head.position, lookTarget, Color.yellow, 0.1f);
            Debug.DrawLine(dogRefs.head.position, finalLookTarget, Color.red, 0.2f);

            var targetRotation = Quaternion.LookRotation(finalLookTarget - dogRefs.head.position);

            // one more thing about which way it should rotate its head. but  it's prob fine. maybe I have better luck in the morning.

            dogRefs.head.rotation = Quaternion.Slerp(dogRefs.head.rotation, targetRotation, lookSmoothness);
        }
        else
        {
            // rotate to identity
            dogRefs.head.rotation = Quaternion.Slerp(dogRefs.head.rotation, dogRefs.headForwardNoRotation.rotation, lookSmoothness);
        }
    }
}
