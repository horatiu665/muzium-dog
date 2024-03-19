using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarStaticNodes : MonoBehaviour
{
    public static List<AStarStaticNodes> allStaticNodes = new List<AStarStaticNodes>();

    public static event System.Action<AStarStaticNodes> OnStaticNodesRemoved;
    public static event System.Action<AStarStaticNodes> OnStaticNodesAdded;

    public AStarSettings aStarSettings;

    public List<AStar.Node> nodes = new List<AStar.Node>();

    public Collider collider => GetComponent<Collider>();

    public Bounds bounds => collider.bounds;

    public float minNodeDist = 1f;

    public float initDelay = 1f;

    void OnEnable()
    {
        allStaticNodes.Add(this);

        StartCoroutine(pTween.Wait(initDelay, () =>
        {
            GenerateNodes();

        }));

    }

    private void OnDisable()
    {
        OnStaticNodesRemoved?.Invoke(this);
        nodes.Clear();

        allStaticNodes.Remove(this);
    }

    public void GenerateNodes()
    {
        OnStaticNodesRemoved?.Invoke(this);
        nodes.Clear();

        var boundsMin = bounds.min;
        var boundsMax = bounds.max;
        for (float x = boundsMin.x; x < boundsMax.x; x += minNodeDist)
        {
            for (float z = boundsMin.z; z < boundsMax.z; z += minNodeDist)
            {
                var pos = new Vector3(x, boundsMax.y, z);
                var sizeY = boundsMax.y - boundsMin.y;
                if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, sizeY, aStarSettings.layerMask))
                {
                    var newNode = new AStar.Node()
                    {
                        // raw nodes are on the exact hit point.
                        position = hit.point,
                        expirationTime = float.MaxValue
                    };
                    nodes.Add(newNode);

                }
            }
        }

        OnStaticNodesAdded?.Invoke(this);
    }



}
