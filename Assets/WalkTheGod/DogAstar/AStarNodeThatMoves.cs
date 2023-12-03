using System.Collections.Generic;
using UnityEngine;

public class AStarNodeThatMoves : MonoBehaviour
{
    public static List<AStarNodeThatMoves> all = new List<AStarNodeThatMoves>();
    public static System.Action<AStarNodeThatMoves> OnNodeAdded, OnNodeRemoved;

    public readonly AStar.Node specialNode = new();

    void OnEnable()
    {
        all.Add(this);
        OnNodeAdded?.Invoke(this);

        specialNode.expirationTime = float.MaxValue;
    }

    void OnDisable()
    {
        OnNodeRemoved?.Invoke(this);
        all.Remove(this);
    }

    private void Update()
    {
        specialNode.position = transform.position;

    }

}