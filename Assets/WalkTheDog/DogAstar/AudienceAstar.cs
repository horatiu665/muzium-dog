using System.Collections.Generic;
using UnityEngine;

// execute  always if you want to edit the staircase nodes in the scene with a little visualization.
[ExecuteAlways]
public class AudienceAstar : MonoBehaviour
{

    /// This clas handles all AStar navigation for the audience at the concert

    public AStar aStar;

    public List<DogConcertAudience> audience = new();

    public bool addNodesOnAudiencePositions = true;

    public Transform staircaseNodesParent;
    [SerializeField, HideInInspector]
    private int staircaseNodesCount;

    private List<Transform> staircaseNodes = new();

    private void FindStaircaseNodes()
    {
        staircaseNodes.Clear();
        foreach (Transform t in staircaseNodesParent)
        {
            staircaseNodes.Add(t);
        }
    }

    private void Start()
    {
        FindStaircaseNodes();

        if (addNodesOnAudiencePositions)
        {
            foreach (var a in audience)
            {
                aStar.AddNode(a.targetAtConcert.position);
                aStar.AddNode(a.targetHidden.position);
            }
        }

        foreach (var s in staircaseNodes)
        {
            aStar.AddNode(s.position);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (staircaseNodesCount != staircaseNodesParent.childCount)
            {
                FindStaircaseNodes();
                staircaseNodesCount = staircaseNodesParent.childCount;
            }
        }
#endif
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var s in staircaseNodes)
        {
            Gizmos.DrawWireSphere(s.position, 0.1f);
        }
    }
}