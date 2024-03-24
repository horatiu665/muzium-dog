using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogLookAttention : MonoBehaviour
{
    public DogRefs dogRefs;

    public DogAstar dogAstar => dogRefs.dogBrain.dogAstar;

    public float lookSmoothness = 0.2f;

    public float headLookAngleLimit = 45;

    public float factor = 3f;

    [Serializable]
    private class LookAtRequest
    {
        public Component whoIsAsking;
        public float priority;
        public int type;
        public Transform target; // 0
        public Vector3 worldPos; // 1
        public Vector3 worldDirection; // 2
        public Vector3 GetLookTargetPosition(DogRefs dog)
        {
            switch (type)
            {
                case 0:
                    return target.position;
                case 1:
                    return worldPos;
                case 2:
                    return dog.head.position + worldDirection;
                default:
                    return dog.head.position + dog.headForwardNoRotation.forward;
            }
        }
    }
    private Dictionary<Component, LookAtRequest> lookAtRequests = new();

    private LookAtRequest currentTopLookRequest;

    [Tooltip("Warning! this might be heavy on performance.")]
    public bool showRequestsDebug = false;

    private void ComputeCurrentTopLookRequest()
    {
        currentTopLookRequest = null;
        foreach (var request in lookAtRequests.Values)
        {
            if (currentTopLookRequest == null || request.priority > currentTopLookRequest.priority)
            {
                currentTopLookRequest = request;
            }
        }
    }

    public void LookAt(Transform target, Component whoIsAsking, float priority = 1)
    {
        if (target == null)
        {
            lookAtRequests.Remove(whoIsAsking);
        }
        else
        {
            lookAtRequests[whoIsAsking] = new LookAtRequest()
            {
                whoIsAsking = whoIsAsking,
                priority = priority,
                type = 0,
                target = target
            };
        }
        // ComputeCurrentTopLookRequest();
    }

    public void LookAtPosition(Vector3 targetPos, Component whoIsAsking, float priority = 1)
    {
        lookAtRequests[whoIsAsking] = new LookAtRequest()
        {
            whoIsAsking = whoIsAsking,
            priority = priority,
            type = 1,
            worldPos = targetPos
        };
        // _currentLookObject = null;
        // lookingAtVector = true;
        // lookTarget = targetPos;
        // ComputeCurrentTopLookRequest();

    }

    public void LookAtDirection(Vector3 dir, Component whoIsAsking, float priority = 1)
    {
        lookAtRequests[whoIsAsking] = new LookAtRequest()
        {
            whoIsAsking = whoIsAsking,
            priority = priority,
            type = 2,
            worldDirection = dir
        };
        // _currentLookObject = null;
        // lookingAtVector = true;
        // lookTarget = dogRefs.headForwardNoRotation.position + dir;
        // ComputeCurrentTopLookRequest();

    }

    void Update()
    {

        // OLD WAY where we only had 1 look target which could be overruled.
        {
            // // if we have a current look object
            // if (_currentLookObject != null && !lookingAtVector)
            // {
            //     // look at the current look object.
            //     lookTarget = _currentLookObject.position;
            // }
            // else if (lookingAtVector)
            // {
            //     // look at the vector. lookTarget already set.
            // }
            // else
            // if (lookAtForward)
            // {
            //     lookTarget = dogRefs.headForwardNoRotation.position + dogRefs.headForwardNoRotation.forward;
            // }
            // // if we're currently moving, look at node[i+1]
            // else if (dogAstar.hasDestination && dogAstar.hasPath)
            // {
            //     var nextNextNode = dogAstar.GetNextNextNode();
            //     if (nextNextNode != null)
            //     {
            //         lookTarget = nextNextNode.position;
            //     }
            // }
            // else
            // {
            //     // look at nothing
            // }
        }

        // dictionary method
        {
            ComputeCurrentTopLookRequest();
        }

        // animate head
        Update_AnimateHead();

    }

    private void Update_AnimateHead()
    {
        if (currentTopLookRequest != null)
        {
            var worldLookTarget = this.currentTopLookRequest.GetLookTargetPosition(dogRefs);
            var localLookTarget = dogRefs.headForwardNoRotation.InverseTransformPoint(worldLookTarget);
            localLookTarget.Normalize();

            // limit head rotation so the dog doesn't break its neck
            var dot = Vector3.Dot(dogRefs.head.forward, dogRefs.headForwardNoRotation.forward);
            if (dot < 0.5f)
            {
                localLookTarget += Vector3.forward * (0.5f - dot) * factor;
            }

            var finalLookTarget = dogRefs.headForwardNoRotation.TransformPoint(localLookTarget);

            Debug.DrawLine(dogRefs.head.position, worldLookTarget, Color.yellow, 0.1f);
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

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (showRequestsDebug)
        {
            GUILayout.BeginArea(new Rect(10, 100, 500, 300));
            GUILayout.Label("LookAtRequests:");
            foreach (var request in lookAtRequests.Values)
            {
                GUILayout.Label(request.whoIsAsking.GetType().Name + " " + request.priority + " " + request.GetLookTargetPosition(dogRefs));
            }
            GUILayout.EndArea();
        }
    }
#endif
}
