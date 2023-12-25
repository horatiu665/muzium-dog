using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogLocomotion : MonoBehaviour
{

    public DogRefs dogRefs;

    public Rigidbody rbRoot;

    public float speed = 20f;
    public float stopDistance = 0.1f;
    public float rotationSpeed = 10;

    public bool hasDestination { get; private set; }
    public bool hasTargetRotation { get; private set; }

    private Vector3 destination;
    private Vector3 targetForward;


    private Vector3 fakeVelocity;
    private Vector3 prevPos;

    public float currentSpeed01
    {
        get
        {
            return fakeVelocity.magnitude / speed;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        hasDestination = true;
    }

    public void SetTargetRotation(Vector3 forward)
    {
        forward.y = 0f;
        forward.Normalize();
        targetForward = forward;
        hasTargetRotation = true;
    }


    public void StopMovement()
    {
        hasDestination = false;
    }
    public void StopRotation()
    {
        hasTargetRotation = false;
    }

    private void FixedUpdate()
    {
        if (hasDestination)
        {
            Vector3 dir = destination - rbRoot.position;

            rbRoot.MovePosition(rbRoot.position + dir.normalized * speed * Time.fixedDeltaTime);

            if (Vector3.Distance(rbRoot.position, destination) < stopDistance)
            {
                StopMovement();
            }
        }

        if (hasTargetRotation)
        {
            Quaternion target = Quaternion.LookRotation(targetForward, Vector3.up);
            rbRoot.MoveRotation(Quaternion.RotateTowards(rbRoot.rotation, target, rotationSpeed * 360f * Time.fixedDeltaTime));
        }

        var fv = (rbRoot.position - prevPos) / Time.fixedDeltaTime;
        fakeVelocity = Vector3.Lerp(fakeVelocity, fv, 0.2f);
        prevPos = rbRoot.position;
    }

    private void OnDrawGizmos()
    {
        if (hasDestination)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(destination, 0.1f);

            Gizmos.color = Color.yellow;
            Vector3 dir = destination - rbRoot.position;
            dir.y = 0f;

            Gizmos.DrawRay(rbRoot.position, dir.normalized * speed);

        }

    }
}
