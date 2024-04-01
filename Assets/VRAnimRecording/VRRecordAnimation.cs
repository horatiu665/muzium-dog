using UnityEngine;

public class VRRecordAnimation : MonoBehaviour
{
    public Transform vrHead, vrLeft, vrRight;

    public VRAnimationData animationData;

    public float recordInterval = 0.1f;

    private float timeSinceLastFrame = 0;

    public bool isRecording = false;

    private void Update()
    {
        if (isRecording)
        {
            timeSinceLastFrame += Time.deltaTime;
            if (timeSinceLastFrame >= recordInterval)
            {
                timeSinceLastFrame = 0;
                VRAnimationData.FullBodyPose pose = new VRAnimationData.FullBodyPose();
                pose.head = new Pose(vrHead.localPosition, vrHead.localRotation);
                pose.leftHand = new Pose(vrLeft.localPosition, vrLeft.localRotation);
                pose.rightHand = new Pose(vrRight.localPosition, vrRight.localRotation);
                animationData.AddKeyframe(pose, Time.time);
            }
        }
    }
}