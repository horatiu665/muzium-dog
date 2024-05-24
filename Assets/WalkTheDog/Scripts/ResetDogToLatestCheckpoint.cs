using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDogToLatestCheckpoint : MonoBehaviour
{
    public DogBrain dogBrain => DogCastleReferences.instance.dogBrain;
    public Transform dog => dogBrain.transform;
    private Rigidbody rb => dogBrain.dogRefs.dogLocomotion.rbRoot;

    private Vector3 checkpointPos;


    public bool resetWhenDogBelowY = true;
    public float resetY = -30;

    [Header("Checkpoints")]
    public bool saveCheckpointWhenDogStandsStill = true;
    public bool saveCheckpointOnlyWithValidGroundedRaycastAndAstarNode = true;
    public float minTimeStandingStill = 5f;
    private float timeStandingStill;

    private void Start()
    {
        checkpointPos = dog.position;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Update()
    {
        if (resetWhenDogBelowY)
        {
            if (dog.position.y < resetY)
            {
                ResetDogPosition();
            }
        }

        if (saveCheckpointWhenDogStandsStill)
        {
            var dogSpeed = dogBrain.dogLocomotion.currentSpeed01;
            if (dogSpeed < 0.02f)
            {
                timeStandingStill += Time.deltaTime;
                if (timeStandingStill > minTimeStandingStill)
                {
                    var newCheckpoint = dog.position;

                    if (saveCheckpointOnlyWithValidGroundedRaycastAndAstarNode)
                    {
                        var nearestNode = dogBrain.dogAstar.aStar.GetNearestNode(newCheckpoint, dogBrain.dogAstar.ignoredNodes);
                        var groundedPos = dogBrain.dogAstar.aStar.GetGroundedPosition(nearestNode.position, out var grounded);
                        if (grounded)
                        {
                            checkpointPos = groundedPos;
                            timeStandingStill = 0;
                        }
                    }
                    else
                    {
                        checkpointPos = dog.position;
                        timeStandingStill = 0;
                    }

                }
            }
            else
            {
                timeStandingStill = 0;
            }
        }
    }

    public void ResetDogPosition()
    {
        dog.position = checkpointPos;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < 3; i++)
        {
            Gizmos.DrawWireSphere(checkpointPos + Vector3.up * i * 0.1f, 0.22f - i * 0.04f);
        }

    }

}
