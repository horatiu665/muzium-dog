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

        public bool ignoreRaycast = false;
    }

    public List<Node> nodes = new List<Node>();

    public DisjointSet disjointSet = new DisjointSet();

    public AStarSettings aStarSettings;
    public LayerMask layerMask => aStarSettings.layerMask;

    public float nodeUpdateInterval = 0.5f;
    private float nodeUpdateTimer;


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
            AddNode(ntm.specialNode);
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
        AddNode(nodeThatMoves.specialNode);

        // nodes.Add(nodeThatMoves.specialNode);
        // AssignNeighbors(nodeThatMoves.specialNode, aStarSettings.neighborDistance);

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
        // nodes.AddRange(staticNodes.nodes);

        var n = staticNodes.nodes.Count;
        for (int i = 0; i < n; i++)
        {
            var node = staticNodes.nodes[i];
            AddNode(node);
        }

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

        for (int i = 0; i < nodes.Count; i++)
        {
            // foreach (var node in nodes)
            var node = nodes[i];
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
        float nearestSqrDistance = float.MaxValue;

        foreach (var node in connectedTo.neighbors)
        {
            // var dist = Vector3.Distance(node.position, destination);
            var sqrDist = (node.position - destination).sqrMagnitude;
            if (sqrDist < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDist;
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

        // easy way to check if there is a path at all. at the expense of the daily neighbor operation
        if (!disjointSet.Connected(startNode, endNode))
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
        int removedCount = 0;
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].expirationTime < Time.time)
            {
                nodes.RemoveAt(i);
                removedCount++;
            }
        }
        if (removedCount > 0)
        {
            RedoNeighbors();
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

        // bad idea. it recalculates far too many nodes.
        // RedoNeighbors();
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

    public Node AddNode(Node existingNode)
    {
        AssignNeighbors(existingNode);

        nodes.Add(existingNode);

        return existingNode;
    }

    public Node AddNode(Vector3 position, float duration = 0, bool recalcNeighbors = true)
    {
        Node newNode = new Node()
        {
            position = position,
            expirationTime = duration > 0 ? Time.time + duration : float.MaxValue,
        };

        AssignNeighbors(newNode);

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
        RedoNeighbors(aStarSettings.neighborDistance);
    }

    public void AssignNeighbors(Node node)
    {
        float maxDistance = aStarSettings.neighborDistance;

        // remove node from neighbors list...
        foreach (var neighbor in node.neighbors)
        {
            neighbor.neighbors.Remove(node);
        }
        // remove node's neighbors.
        node.neighbors.Clear();

        disjointSet.Add(node);

        // redo neighbors only for this node
        var maxSqrDist = maxDistance * maxDistance;
        var nodesCount = nodes.Count;
        for (int j = 0; j < nodesCount; j++)
        {
            var otherNode = nodes[j];
            if (otherNode == node)
                continue;

            var sqrDist = (node.position - otherNode.position).sqrMagnitude;
            if (sqrDist < maxSqrDist)
            {
                // if nodes are not ignoring raycast, and if there is a raycast hit, don't add them as neighbors.
                // raycast. if it's blocked, don't add
                if (!(node.ignoreRaycast || otherNode.ignoreRaycast)
                    && Physics.Raycast(node.position + Vector3.up * nodeVerticalOffset, otherNode.position - node.position, out RaycastHit hit, Mathf.Sqrt(sqrDist), layerMask))
                {
                    continue;
                }
                node.neighbors.Add(otherNode);
                otherNode.neighbors.Add(node);
                disjointSet.Union(node, otherNode);
            }
        }
    }

    public void RedoNeighbors(float maxDistance)
    {
        var nodesCount = nodes.Count;
        var maxSqrDist = maxDistance * maxDistance;
        // start with clearing neighbors.
        for (int i = 0; i < nodesCount; i++)
        {
            nodes[i].neighbors.Clear();
        }
        // reset disjoint set
        disjointSet.Clear();
        disjointSet.Add(nodes);

        // check each pair of nodes.
        for (int i = 0; i < nodesCount; i++)
        {
            var node = nodes[i];

            // foreach (var node in nodes)
            // {
            // node.neighbors.Clear();
            for (int j = i + 1; j < nodesCount; j++)
            {
                var otherNode = nodes[j];

                // foreach method (too slow to GetNext())
                // foreach (var otherNode in nodes)
                // {
                // if (node == otherNode)
                // {
                //     continue;
                // }


                // var dist = Vector3.Distance(node.position, otherNode.position);
                // if (dist < maxDistance)
                var sqrDist = (node.position - otherNode.position).sqrMagnitude;
                if (sqrDist < maxSqrDist)
                {
                    // if nodes are not ignoring raycast, and if there is a raycast hit, don't add them as neighbors.
                    // raycast. if it's blocked, don't add
                    if (!(node.ignoreRaycast || otherNode.ignoreRaycast)
                        && Physics.Raycast(node.position + Vector3.up * nodeVerticalOffset, otherNode.position - node.position, out RaycastHit hit, Mathf.Sqrt(sqrDist), layerMask))
                    {
                        continue;
                    }
                    node.neighbors.Add(otherNode);
                    otherNode.neighbors.Add(node);
                    disjointSet.Union(node, otherNode);
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

            // RedoNeighbors(neighborDistance);
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

    public Node GetNearestNodeToLine(Vector3 position, Vector3 forward, List<Node> exceptNodes)
    {
        Node nearestNode = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < nodes.Count; i++)
        {
            // foreach (var node in nodes)
            var node = nodes[i];
            if (exceptNodes.Contains(node))
                continue;

            // calculate distance to line that passes through position and has direction forward
            var dist = Vector3.Cross(forward, node.position - position).magnitude;
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestNode = node;
            }
        }

        return nearestNode;

    }
}
