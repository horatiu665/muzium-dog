using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarCameraDebug : MonoBehaviour
{
    public AStar aStar;

    private AStar.Node nearestNode;

    void Update()
    {
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f, aStar.layerMask))
            {
                // find nearest node
                nearestNode = aStar.GetNearestNode(hit.point);


            }
        }
    }

    private void OnDrawGizmos()
    {
        if (nearestNode != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(nearestNode.position, 0.1f);

            Gizmos.color = Color.red;
            // draw its neighbors
            foreach (var neighbor in nearestNode.neighbors)
            {
                Gizmos.DrawLine(nearestNode.position, neighbor.position + Vector3.up * 0.1f);
            }

        }
    }
}
