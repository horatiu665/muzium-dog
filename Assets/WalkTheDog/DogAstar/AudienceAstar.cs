using System.Collections.Generic;
using UnityEngine;

public class AudienceAstar : MonoBehaviour
{

    /// This clas handles all AStar navigation for the audience at the concert


    public AStar aStar;

    public List<DogConcertAudience> audience = new();

    public bool addNodesOnAudiencePositions = true;

    public List<Transform> staircaseNodes = new();

    private void Start()
    {
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        foreach (var s in staircaseNodes)
        {
            Gizmos.DrawWireSphere(s.position, 0.1f);
        }
    }
}