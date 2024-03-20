using UnityEngine;

[CreateAssetMenu(fileName = "AStarSettings", menuName = "Dog/AStarSettings", order = 0)]
public class AStarSettings : ScriptableObject
{

    public LayerMask layerMask = -1;
    public float nodeVerticalOffset = 0.1f;

    public float neighborDistance = 5f;

    public bool redoNeighborsOnPath = false;

}