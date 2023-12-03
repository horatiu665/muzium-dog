using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DogAstar : MonoBehaviour
{
    public AStar aStar;
    public DogLocomotion dogLocomotion;

    public Vector3 destination;
    public float stopDistance = 1f;
    public bool hasDestination { get; private set; }

    public bool hasPath { get; private set; }
    private List<AStar.Node> _path = new List<AStar.Node>();

    public float rareNodeRaycastCheck = 0.2f;
    private float rareNodeRaycastLastTime;

    private AStar.Node startNode, endNode;

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        hasDestination = true;
        hasPath = false;

    }

    public void StopMovement()
    {
        hasDestination = false;
        dogLocomotion.StopMovement();
    }

    private void Update()
    {
        if (hasDestination)
        {
            if (Vector3.Distance(transform.position, destination) < stopDistance)
            {
                StopMovement();
            }

            if (hasPath)
            {
                // move to next node
                if (_path.Count > 0)
                {
                    // skip node if the second next node is in clear sight
                    if (Time.time > rareNodeRaycastLastTime + rareNodeRaycastCheck)
                    {
                        rareNodeRaycastLastTime = Time.time;
                        if (_path.Count > 1)
                        {
                            var dirToPath1 = _path[1].position - transform.position;
                            // if no obstacle detected, remove [0].
                            if (!Physics.Raycast(transform.position, dirToPath1, out RaycastHit hit, dirToPath1.magnitude, aStar.layerMask))
                            {
                                _path.RemoveAt(0);
                            }
                        }
                    }

                    Vector3 dir = _path[0].position - transform.position;

                    dogLocomotion.SetDestination(transform.position + dir);
                    dogLocomotion.SetTargetRotation(dir);
                    // skip node if close enough
                    if (dir.magnitude < stopDistance)
                    {
                        _path.RemoveAt(0);
                    }

                }
                else
                {
                    hasPath = false;
                }
            }
            else
            {
                // get path
                if (startNode == null)
                {
                    startNode = aStar.AddNode(transform.position);
                }
                else
                {
                    startNode.position = transform.position;
                }
                if (endNode == null)
                {
                    endNode = aStar.AddNode(destination);
                }
                else
                {
                    endNode.position = destination;
                }
                // consider how to optimize the RedoNeighbors....?!?!
                aStar.RedoNeighbors();

                _path = aStar.GetPath(startNode, endNode);
                if (_path == null)
                {
                    hasPath = false;
                    // can't find a path.
                    CantFindAPath();
                }
                else
                {
                    hasPath = true;
                }

            }
        }

    }

    void CantFindAPath()
    {
        // obvious option: stop movement. can't go there, stop trying.
        // StopMovement();

        // alternative option: find the nearest point on the nodes map to the destination, and go there.
        {
            // first abandon the end node so it doesn't interfere
            endNode.position = startNode.position;
            var nearestNode = aStar.GetNearestNeighborNode(destination, startNode, 10);
            if (nearestNode == null)
            {
                StopMovement();
            }
            else
            {
                SetDestination(nearestNode.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (hasPath)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Gizmos.DrawLine(_path[i].position, _path[i + 1].position);
            }
        }

        if (hasDestination)
        {
            Gizmos.color = hasPath ? Color.yellow : Color.red;
            Gizmos.DrawWireSphere(destination, 0.1f);
        }
    }

}