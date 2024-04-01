using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianistFollowAnimation : MonoBehaviour
{
    public VRAnimationData animationData;

    public float speed = 1.0f;

    public float currentTime = 0;

    public Transform headIK, leftHandIK, rightHandIK;

    private void Update() 
    {
        currentTime += Time.deltaTime * speed;
        VRAnimationData.Keyframe prevKeyframe = animationData.GetPrevKeyframe(currentTime);
        VRAnimationData.Keyframe nextKeyframe = animationData.GetNextKeyframe(prevKeyframe);

        if (prevKeyframe == null || nextKeyframe == null)
        {
            return;
        }

        float t = (currentTime - prevKeyframe.time) / (nextKeyframe.time - prevKeyframe.time);
        var lerpPose = VRAnimationData.FullBodyPose.Lerp(prevKeyframe.pose, nextKeyframe.pose, t);

        headIK.SetPositionAndRotation(lerpPose.head.position, lerpPose.head.rotation);
        leftHandIK.SetPositionAndRotation(lerpPose.leftHand.position, lerpPose.leftHand.rotation);
        rightHandIK.SetPositionAndRotation(lerpPose.rightHand.position, lerpPose.rightHand.rotation);
        
    }
    
}
