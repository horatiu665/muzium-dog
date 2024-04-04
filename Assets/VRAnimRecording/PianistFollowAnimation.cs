using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianistFollowAnimation : MonoBehaviour
{
    public VRAnimationData animationData;

    public bool isPlaying;

    public float speed = 1.0f;

    public float currentTime = 0;

    public Transform headIK, leftHandIK, rightHandIK;

    [Header("settings for ik simulation")]
    public Transform torsoRef;
    public Transform rootFwdRef;
    public Transform headRef;
    public Transform leftArmRef, rightArmRef;


    private void Update()
    {
        if (!isPlaying)
            return;

        currentTime += Time.deltaTime * speed;
        if (currentTime > animationData.GetTotalTime())
        {
            currentTime -= animationData.GetTotalTime();
        }
        VRAnimationData.Keyframe prevKeyframe = animationData.GetPrevKeyframe(currentTime);
        VRAnimationData.Keyframe nextKeyframe = animationData.GetNextKeyframe(prevKeyframe);

        if (prevKeyframe == null || nextKeyframe == null)
        {
            return;
        }

        float t = (currentTime - prevKeyframe.time) / (nextKeyframe.time - prevKeyframe.time);
        var lerpPose = VRAnimationData.FullBodyPose.Lerp(prevKeyframe.pose, nextKeyframe.pose, t);

        lerpPose.SetLocalPositionsToTargets(headIK, leftHandIK, rightHandIK);
        
        // handle ik simulation
        torsoRef.LookAt(headIK.position, rootFwdRef.forward);
        headRef.rotation = headIK.rotation;
        leftArmRef.LookAt(leftHandIK.position, Vector3.up);
        rightArmRef.LookAt(rightHandIK.position, Vector3.up);

    }

}
