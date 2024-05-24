using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
///  Uses AStar and the DogLocomotion system to move the dog on a path.
/// </summary>
public class DogAstar : MonoBehaviour
{
    public AStar aStar;
    public DogLocomotion dogLocomotion;

    public Vector3 destination;
    public float stopDistance = 1f;
    public bool hasDestination { get; private set; }

    public bool hasPath { get; private set; }
    private List<AStar.Node> _path = new List<AStar.Node>();

    private AStar.Node _prevVisitedNode;

    public float rareNodeRaycastCheck = 0.2f;
    private float rareNodeRaycastLastTime;

    public bool startNodeIgnoresRaycast = false;

    private AStar.Node startNode, endNode;

    public float cantFindPathTime;
    public float lastPathCalculationTime;

    public readonly List<AStar.Node> ignoredNodes = new();

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

    private void OnEnable()
    {
        dogLocomotion.OnNextPositionNotGrounded += OnNextPositionNotGrounded;
    }

    private void OnDisable()
    {
        dogLocomotion.OnNextPositionNotGrounded -= OnNextPositionNotGrounded;
    }

    private void OnNextPositionNotGrounded()
    {
        if (_path == null)
            return;

        // previous node where we're coming from
        var prevNode = _prevVisitedNode;
        var nextNode = GetNextNode();
        if (nextNode != null && _prevVisitedNode != null)
        {
            // remove neighbor
            prevNode.neighbors.Remove(nextNode);
            nextNode.neighbors.Remove(prevNode);

            // recalculate path.
            hasPath = false;
        }

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
                            // if no obstacle detected, remove [0].
                            if (!FindObstacle(transform.position, _path[1].position))
                            {
                                _path.RemoveAt(0);
                            }
                        }
                    }

                    // Vector3 dir = _path[0].position - transform.position;
                    Vector3 dir = _path[0].position - transform.position;

                    dogLocomotion.SetDestination(transform.position + dir);
                    // rotate towards next node
                    dogLocomotion.SetTargetRotation(dir);

                    // skip node if close enough
                    if (dir.magnitude < stopDistance)
                    {
                        _prevVisitedNode = _path[0];
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
                    ignoredNodes.Add(startNode);
                    if (startNodeIgnoresRaycast)
                    {
                        // an attempt to reduce the moments when it got stuck. but it might cause glitches thru walls n stuff.
                        startNode.ignoreRaycast = true;
                    }

                }
                else
                {
                    startNode.position = transform.position;
                    aStar.AssignNeighbors(startNode);
                }

                if (endNode == null)
                {
                    // node position should be exactly the destination (but destination should be reachable. we assume this in SetDestination()).
                    endNode = aStar.AddNode(destination);
                    ignoredNodes.Add(endNode);
                }
                else
                {
                    endNode.position = destination;
                    aStar.AssignNeighbors(endNode);

                }
                // consider how to optimize the RedoNeighbors....?!?!
                // it's def bad to do it every time we do a new path.
                if (aStar.aStarSettings.redoNeighborsOnPath)
                {
                    aStar.RedoNeighbors();
                }

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
                    lastPathCalculationTime = Time.time;
                }

            }
        }

    }

    public AStar.Node GetNextNode()
    {
        if (_path.Count > 0)
        {
            return _path[0];
        }
        return null;
    }

    public AStar.Node GetNextNextNode()
    {
        if (_path.Count > 1)
        {
            return _path[1];
        }
        return null;
    }

    private bool FindObstacle(Vector3 pointA, Vector3 pointB)
    {
        var dir = pointB - pointA;
        return Physics.Raycast(pointA, dir, out RaycastHit hit, dir.magnitude, aStar.layerMask);
    }

    void CantFindAPath()
    {
        cantFindPathTime = Time.time;

        // obvious option: stop movement. can't go there, stop trying.
        // StopMovement();

        // alternative option: find the nearest point on the nodes map to the destination, and go there.
        {
            // maybe we don't have to remove the end node since it won't be connected.
            //aStar.RemoveNode(endNode);

            var nearestConnectedNode = aStar.GetNearestConnectedNode(destination, startNode);
            SetDestination(nearestConnectedNode.position);

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
            Gizmos.DrawWireSphere(destination, 0.08f);
        }
    }

}