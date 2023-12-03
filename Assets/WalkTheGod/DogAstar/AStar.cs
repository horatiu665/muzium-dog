using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public class Node
    {
        public Vector3 position;
        public List<Node> neighbors = new List<Node>();

        // gCost is the cost of getting from the start node to this node
        public float gCost;
        // hCost is the cost of getting from this node to the end node
        public float hCost;
        public float fCost => gCost + hCost;

        public Node previous;

        public float expirationTime;
    }

    public List<Node> nodes = new List<Node>();

    public AStarSettings aStarSettings;
    public LayerMask layerMask => aStarSettings.layerMask;

    public float nodeUpdateInterval = 0.5f;
    private float nodeUpdateTimer;

    public float neighborDistance = 5f;

    List<Node> openSet = new List<Node>();
    HashSet<Node> closedSet = new HashSet<Node>();
    List<Node> cancelledNodes = new List<Node>();

    public float nodeVerticalOffset => aStarSettings.nodeVerticalOffset;

    private void OnEnable()
    {
        AStarStaticNodes.OnStaticNodesRemoved += OnStaticNodesRemoved;
        AStarStaticNodes.OnStaticNodesAdded += OnStaticNodesAdded;

        // find all moving nodes
        foreach (var ntm in AStarNodeThatMoves.all)
        {
            nodes.Add(ntm.specialNode);
        }
        AStarNodeThatMoves.OnNodeAdded += OnMovingNodeAdded;
        AStarNodeThatMoves.OnNodeRemoved += OnMovingNodeRemoved;
    }

    private void OnDisable()
    {
        AStarStaticNodes.OnStaticNodesRemoved -= OnStaticNodesRemoved;
        AStarStaticNodes.OnStaticNodesAdded -= OnStaticNodesAdded;

        AStarNodeThatMoves.OnNodeAdded -= OnMovingNodeAdded;
        AStarNodeThatMoves.OnNodeRemoved -= OnMovingNodeRemoved;
    }

    private void OnMovingNodeAdded(AStarNodeThatMoves nodeThatMoves)
    {
        nodes.Add(nodeThatMoves.specialNode);
    }

    private void OnMovingNodeRemoved(AStarNodeThatMoves nodeThatMoves)
    {
        nodes.Remove(nodeThatMoves.specialNode);
    }

    private void OnStaticNodesRemoved(AStarStaticNodes staticNodes)
    {
        nodes.RemoveAll(n => staticNodes.nodes.Contains(n));
    }

    private void OnStaticNodesAdded(AStarStaticNodes staticNodes)
    {
        nodes.AddRange(staticNodes.nodes);

    }

    public Node GetNearestNodeWithClearView(Vector3 position)
    {
        Node nearestNode = null;
        float nearestDistance = float.MaxValue;
        cancelledNodes.Clear();
        bool found = false;
        int triesLeft = 10;

        while (!found && triesLeft-- > 0)
        {
            foreach (var node in nodes)
            {
                if (cancelledNodes.Contains(node))
                {
                    continue;
                }

                var dist = Vector3.Distance(node.position, position);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestNode = node;
                }
            }

            // raycast to nearest node
            if (Physics.Raycast(position + Vector3.up * nodeVerticalOffset, nearestNode.position - position, out RaycastHit hit, nearestDistance, layerMask))
            {
                cancelledNodes.Add(nearestNode);
            }
            else
            {
                found = true;
            }
        }

        return nearestNode;

    }
    public Node GetNearestNode(Vector3 position)
    {
        Node nearestNode = null;
        float nearestDistance = float.MaxValue;

        foreach (var node in nodes)
        {
            var dist = Vector3.Distance(node.position, position);
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestNode = node;
            }
        }

        return nearestNode;
    }
    public Node GetNearestNeighborNode(Vector3 destination, Node connectedTo, int iterations = 10)
    {
        Node nearestNode = null;
        float nearestDistance = float.MaxValue;

        foreach (var node in connectedTo.neighbors)
        {
            var dist = Vector3.Distance(node.position, destination);
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestNode = node;
            }
        }

        if (iterations <= 0 || nearestNode == null)
        {
            return nearestNode;
        }

        return GetNearestNeighborNode(destination, nearestNode, iterations - 1);

    }

    public List<Node> GetPath(Node startNode, Node endNode)
    {
        if (startNode == null || endNode == null)
        {
            return null;
        }

        openSet.Clear();
        closedSet.Clear();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (Node neighbor in currentNode.neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                var delta = neighbor.position - currentNode.position;
                var dist = delta.magnitude;
                // var dist = new Vector2(delta.x, delta.z).magnitude + Mathf.Abs(delta.y) * 5f;
                var movementCost = dist;

                // check raycast here. if it's blocked, then don't add it to the open set
                if (Physics.Raycast(currentNode.position + Vector3.up * nodeVerticalOffset, neighbor.position - currentNode.position, out RaycastHit hit, delta.magnitude, layerMask))
                {
                    movementCost *= 1000;
                }

                float newMovementCostToNeighbor = currentNode.gCost + movementCost;

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = Vector3.Distance(neighbor.position, endNode.position);
                    neighbor.previous = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.previous;
        }

        path.Reverse();

        return path;
    }

    public void RemoveOldNodes()
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].expirationTime < Time.time)
            {
                nodes.RemoveAt(i);
            }
        }
    }

    public void CreateNodesNearby(Vector3 center, float radius, int count, float minDistance)
    {
        for (int i = 0; i < count; i++)
        {
            // get random position
            var rc = Random.insideUnitCircle;
            Vector3 randomPos = center + new Vector3(rc.x, 0, rc.y) * radius;

            var groundedPos = GetGroundedPosition(randomPos, out var grounded);
            if (!grounded)
            {
                continue;
            }

            bool tooClose = IsTooCloseToAnotherNode(groundedPos, minDistance);

            if (tooClose)
            {
                continue;
            }

            AddNode(groundedPos);
        }
    }

    private bool IsTooCloseToAnotherNode(Vector3 pos, float minDistance)
    {
        // check if it's too close to any other node
        bool tooClose = false;
        foreach (var node in nodes)
        {
            if (Vector3.Distance(node.position, pos) < minDistance)
            {
                tooClose = true;
                break;
            }
        }

        return tooClose;
    }

    public Node AddNode(Vector3 position, float duration = 0)
    {
        Node newNode = new Node()
        {
            position = position,
            expirationTime = duration > 0 ? Time.time + duration : float.MaxValue,
        };

        nodes.Add(newNode);

        return newNode;
    }

    public Vector3 GetGroundedPosition(Vector3 position, out bool grounded, float raycastHeight = 3f, float raycastDistance = 6f)
    {
        if (Physics.Raycast(position + Vector3.up * raycastHeight, Vector3.down, out RaycastHit hit, raycastDistance, layerMask))
        {
            grounded = true;
            return hit.point + Vector3.up * aStarSettings.nodeVerticalOffset;
        }

        grounded = false;
        return position;
    }

    public void RedoNeighbors()
    {
        RedoNeighbors(neighborDistance);
    }

    public void RedoNeighbors(float maxDistance)
    {
        foreach (var node in nodes)
        {
            node.neighbors.Clear();
            foreach (var otherNode in nodes)
            {
                if (node == otherNode)
                {
                    continue;
                }

                var dist = Vector3.Distance(node.position, otherNode.position);
                if (dist < maxDistance)
                {
                    // raycast. if it's blocked, don't add
                    if (Physics.Raycast(node.position + Vector3.up * nodeVerticalOffset, otherNode.position - node.position, out RaycastHit hit, dist, layerMask))
                    {
                        continue;
                    }
                    node.neighbors.Add(otherNode);
                }
            }
        }

    }

    void Update()
    {
        if (nodeUpdateTimer <= Time.time)
        {
            nodeUpdateTimer = Time.time + nodeUpdateInterval;

            RemoveOldNodes();

            RedoNeighbors(neighborDistance);
        }
    }
    void OnDrawGizmos()
    {
        foreach (var node in nodes)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(node.position, 0.1f);

            foreach (var neighbor in node.neighbors)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(node.position, neighbor.position);
            }
        }
    }

}
