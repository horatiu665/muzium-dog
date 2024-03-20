using UnityEngine;

[CreateAssetMenu(fileName = "DogBarkSettings", menuName = "Dog/DogBarkSettings", order = 0)]
public class DogBarkSettings : ScriptableObject
{
    [TextArea]
    public string notes;

    // moves dog up depending on envelope
    public float upAmount = 1f;
    public AnimationCurve upCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(1, 1)
    );

    // shakes dog in all directions depending on envelope
    public float shakeAmount = 0.5f;
    public AnimationCurve shakeCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(1, 1)
    );

    // smooths out the movement
    public float smoothness = 0.5f;
}