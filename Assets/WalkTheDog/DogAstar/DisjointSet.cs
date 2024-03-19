using System.Collections.Generic;
using Node = AStar.Node;

public class DisjointSet
{
    private Dictionary<Node, Node> parent = new Dictionary<Node, Node>();

    public DisjointSet()
    {
    }

    public void Clear()
    {
        parent.Clear();
    }

    public void Add(Node node)
    {
        parent[node] = node;
    }

    public void Add(List<Node> nodes)
    {
        foreach (var node in nodes)
        {
            Add(node);
        }
    }

    public Node Find(Node node)
    {
        if (!parent[node].Equals(node))
        {
            parent[node] = Find(parent[node]);  // Path compression
        }
        return parent[node];
    }

    public void Union(Node node1, Node node2)
    {
        Node root1 = Find(node1);
        Node root2 = Find(node2);
        if (!root1.Equals(root2))
        {
            parent[root1] = root2;
        }
    }

    public bool Connected(Node node1, Node node2)
    {
        return Find(node1).Equals(Find(node2));
    }
}