using UnityEngine;

public class AStarNodesAroundMovingTarget : MonoBehaviour
{
    public AStar aStar;

    public float addNodesDelay = 3;
    private float addNodesLastTime;

    [Space]
    public float nodeNeighborDistance = 5;
    public float newNodeRadius = 20;
    public int newNodeCount = 20;

    public float newNodeDuration = 0;

    void Update()
    {
        Update_AddNodes();
    }

    private void Update_AddNodes()
    {
        if (Time.time > addNodesLastTime + addNodesDelay)
        {
            addNodesLastTime = Time.time;

            // add nodes
            aStar.CreateNodesNearby(transform.position, newNodeRadius, newNodeCount, nodeNeighborDistance * 0.5f, newNodeDuration);

        }
    }

}