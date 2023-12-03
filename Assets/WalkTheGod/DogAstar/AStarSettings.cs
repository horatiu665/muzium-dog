using UnityEngine;

[CreateAssetMenu(fileName = "AStarSettings", menuName = "AStarSettings", order = 0)]
public class AStarSettings : ScriptableObject
{

    public LayerMask layerMask = -1;
    public float nodeVerticalOffset = 0.1f;

}