using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarStaticNodes : MonoBehaviour
{
    public static List<AStarStaticNodes> allStaticNodes = new List<AStarStaticNodes>();

    public static event System.Action<AStarStaticNodes> OnStaticNodesRemoved;
    public static event System.Action<AStarStaticNodes> OnStaticNodesAdded;

    [Header("use TransparentFX layer so collider is out of the way.")]
    public AStarSettings aStarSettings;

    public bool disableMeshRendererOnEnable = true;

    public List<AStar.Node> nodes = new List<AStar.Node>();

    public Collider collider => GetComponent<Collider>();

    public Bounds bounds => collider.bounds;

    public float minNodeDist = 1f;

    [Header("Generates nodes after this delay.")]
    public float initDelay = 1f;

    public bool showGizmos = true;
    public Color gizmosColor = Color.red * Color.yellow;

    void OnEnable()
    {
        if (disableMeshRendererOnEnable)
        {
            var mr = GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.enabled = false;
            }
        }

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
        
        if (minNodeDist < 0.02f)
        {
            minNodeDist = 0.02f;
        }

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


    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            if (minNodeDist < 0.02f)
            {
                minNodeDist = 0.02f;
            }

            Gizmos.color = gizmosColor;

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
                        Gizmos.DrawLine(pos, hit.point);
                        Gizmos.DrawWireSphere(hit.point, 0.1f);
                    }
                }
            }
        }
    }
}
