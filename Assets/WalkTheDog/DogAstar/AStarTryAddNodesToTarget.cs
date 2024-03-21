using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarTryAddNodesUnderPlayer : MonoBehaviour
{


    [SerializeField]
    private DogRefs _dogRefs;
    public DogRefs dogRefs
    {
        get
        {
            if (_dogRefs == null)
            {
                _dogRefs = GetComponentInParent<DogRefs>();
            }
            return _dogRefs;
        }
    }

    private FirstPersonController fpc => dogRefs.dogBrain.playerFPC;

    private Vector3 prevPosChecked;

    public float minDistanceToCheck = 1;

    public float nodeDuration = 30f;
    // private List<AStar.Node> allAddedNodes = new List<AStar.Node>();

    private void OnEnable()
    {
        var p = dogRefs.dogBrain.player;
        if (p != null)
        {
            CheckPositionAndAddNode(p.position);
        }
    }

    void Update()
    {
        var p = dogRefs.dogBrain.player;
        if (p != null)
        {
            if (fpc != null) // we have Zium player
            {
                // only if player is grounded
                if (fpc.isGrounded)
                {
                    CheckPositionAndAddNode(p.position);
                }
            }
            else // we have regular player
            {
                CheckPositionAndAddNode(p.position);

            }
        }

    }

    private void CheckPositionAndAddNode(Vector3 playerPosition)
    {
        // check if there is a node near playerPosition in the aStar, that's closer than minDistanceToCheck.
        // if not, add a node there.
        if (Vector3.Distance(prevPosChecked, playerPosition) > minDistanceToCheck)
        {
            prevPosChecked = playerPosition;
            var aStar = dogRefs.dogBrain.dogAstar.aStar;
            var node = aStar.GetNearestNode(playerPosition);
            if (node == null || Vector3.Distance(node.position, playerPosition) > minDistanceToCheck)
            {
                var newNode = aStar.AddNode(playerPosition, nodeDuration);
                Debug.DrawRay(playerPosition, Vector3.up * 0.5f, Color.red, 30);
                // allAddedNodes.Add(newNode);
            }
        }

    }
}
