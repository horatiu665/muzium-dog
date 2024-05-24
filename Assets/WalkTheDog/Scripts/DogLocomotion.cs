using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogLocomotion : MonoBehaviour
{

    public DogRefs dogRefs;

    public Rigidbody rbRoot;

    // this lets the DogLocomotion control rigidbody.isKinematic status. otherwise, it remains set from the inspector.
    public bool allowKinematicControl = true;

    public bool doOwnGravity = true;
    public float groundedCastRadius = 0.5f;
    public float teleportToNodeAboveMaxDistanceIfNotGrounded = 3f;

    public GameObject groundedDebugThing;
    public Collider groundedUponThis;

    public float topSpeed = 20f;
    public float targetSpeed01 = 1f;

    public float stopDistance = 0.1f;
    public float rotationSpeed = 10;
    public float rotationLerp = 0.1f;
    private float howManyFriendsOfDog = 1;

    public bool hasDestination { get; private set; }
    public bool hasTargetRotation { get; private set; }

    private Vector3 destination;
    private Vector3 targetForward;


    private Vector3 fakeVelocity;
    private Vector3 prevPos;


    // triggered when the next position we are meant to move to is not grounded. useful to readjust the path.
    public event System.Action OnNextPositionNotGrounded;

    /// <summary>
    /// Calculated as fakeVelocity / topSpeed, based on actual position delta between frames.
    /// </summary>
    public float currentSpeed01
    {
        get
        {
            return fakeVelocity.magnitude / topSpeed;
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

    public void SetKinematic(bool isKinematic)
    {
        if (allowKinematicControl)
        {
            rbRoot.isKinematic = isKinematic;
        }
    }

    private void FixedUpdate()
    {
        if (doOwnGravity)
        {
            // find out if we are grounded with a sphere cast
            RaycastHit hit;
            if (Physics.SphereCast(rbRoot.position + Vector3.up * (groundedCastRadius + 0.1f),
                groundedCastRadius, Vector3.down, out hit, groundedCastRadius * 2 + 0.1f, dogRefs.dogBrain.dogAstar.aStar.aStarSettings.layerMask))
            {
                groundedDebugThing.SetActive(true);
                groundedUponThis = hit.collider;
            }
            else
            {
                groundedDebugThing.SetActive(false);
                groundedUponThis = null;

                // only move up to the nearest node if we are kinematic. if we are physics based we might as well fall off.
                if (rbRoot.isKinematic)
                {
                    // if we have a node above within the teleportToNodeAboveMaxDistanceIfNotGrounded distance, move to it
                    var nodeAbove = dogRefs.dogBrain.dogAstar.aStar.GetNearestNode(rbRoot.position, dogRefs.dogBrain.dogAstar.ignoredNodes);
                    if (nodeAbove != null)
                    {
                        // debug to see when this crazy thing happens. 
                        // Debug.Log("NodeAbove!");
                        // Debug.DrawLine(Vector3.zero, nodeAbove.position + Random.onUnitSphere * 0.1f, Color.white, 0.15f);

                        var distToNodeAbove = Vector3.Distance(nodeAbove.position, rbRoot.position);
                        if (distToNodeAbove < teleportToNodeAboveMaxDistanceIfNotGrounded)
                        {
                            // move on Y axis
                            var targetPos = rbRoot.position;
                            // reversed gravity (x2) so it falls upward to save itself...?!!?!
                            targetPos.y = Mathf.MoveTowards(rbRoot.position.y, targetPos.y, 2 * Physics.gravity.y * Time.fixedDeltaTime);
                            rbRoot.MovePosition(targetPos);

                        }

                    }
                    else
                    // we are airborne
                    {
                        rbRoot.MovePosition(rbRoot.position + Physics.gravity * Time.fixedDeltaTime);
                    }

                }

            }

        }

        if (hasDestination)
        {
            Vector3 dir = destination - rbRoot.position;

            if (!rbRoot.isKinematic)
            {
                rbRoot.velocity *= 0.6f;
                rbRoot.angularVelocity *= 0.6f;
            }

            var nextPosition = rbRoot.position + dir.normalized * topSpeed * targetSpeed01 * Time.fixedDeltaTime;

            // if next position is floating in the air.... avoid it??? how? raycast?
            var nextPositionGrounded = dogRefs.dogBrain.dogAstar.aStar.GetGroundedPosition(nextPosition, out bool isNextPosGrounded);
            if (!isNextPosGrounded)
            {
                // what should we do when next pos is not grounded?
                OnNextPositionNotGrounded?.Invoke();

            }
            else
            {
                rbRoot.MovePosition(nextPosition);

                if (Vector3.Distance(rbRoot.position, destination) < stopDistance)
                {
                    StopMovement();
                }
            }
        }
        else
        {
            // slow down cause we don't have a destination
            // if (!rbRoot.isKinematic)
            // {
            //     rbRoot.velocity *= 0.9f;
            // }
        }

        if (hasTargetRotation)
        {

            Quaternion target = Quaternion.LookRotation(targetForward, Vector3.up);
            // var lerpTarget = Quaternion.RotateTowards(rbRoot.rotation, target, rotationSpeed * 360f * Time.fixedDeltaTime);
            var lerpTarget = Quaternion.Slerp(rbRoot.rotation, target, rotationLerp);
            rbRoot.MoveRotation(lerpTarget);

            // Debug.DrawRay(transform.position, targetForward * 10, Color.red, 0.5f);

            if (!rbRoot.isKinematic)
            {
                rbRoot.angularVelocity *= 0.6f;
            }

            // when rotated, stop having a target rotation.
            if (Quaternion.Angle(rbRoot.rotation, target) < 1f)
            {
                StopRotation();
            }
        }
        else
        {
            // slow down rotation cause we don't have a target rotation
            if (!rbRoot.isKinematic)
            {
                rbRoot.angularVelocity *= 0.9f;
            }
        }

        var fv = (rbRoot.position - prevPos) / Time.fixedDeltaTime;
        fakeVelocity = Vector3.Lerp(fakeVelocity, fv, 0.2f);
        prevPos = rbRoot.position;
    }

    private void OnDrawGizmos()
    {
        if (hasDestination)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(destination, 0.05f);
            Gizmos.DrawLine(rbRoot.position, destination);

            Gizmos.color = Color.yellow;
            Vector3 dir = destination - rbRoot.position;
            dir.y = 0f;

            Gizmos.DrawRay(rbRoot.position, dir.normalized * topSpeed * targetSpeed01);

        }

    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (allowKinematicControl)
        {
            // set gui color
            GUI.color = rbRoot.isKinematic ? Color.green : Color.red;
            GUI.Label(new Rect(10, 30, 200, 20), "Dog Kinematic: " + rbRoot.isKinematic);

            // reset gui color
            GUI.color = Color.white;
        }

    }
#endif
}
