using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VRAnimationData", menuName = "Dog/VRAnimationData", order = 0)]
public class VRAnimationData : ScriptableObject
{
    public struct FullBodyPose
    {
        public Pose head, leftHand, rightHand;

        public static FullBodyPose Lerp(FullBodyPose a, FullBodyPose b, float t)
        {
            FullBodyPose pose = new FullBodyPose();
            pose.head = new Pose(Vector3.Lerp(a.head.position, b.head.position, t), Quaternion.Slerp(a.head.rotation, b.head.rotation, t));
            pose.leftHand = new Pose(Vector3.Lerp(a.leftHand.position, b.leftHand.position, t), Quaternion.Slerp(a.leftHand.rotation, b.leftHand.rotation, t));
            pose.rightHand = new Pose(Vector3.Lerp(a.rightHand.position, b.rightHand.position, t), Quaternion.Slerp(a.rightHand.rotation, b.rightHand.rotation, t));
            return pose;
        }
    }

    public class Keyframe
    {
        public FullBodyPose pose;
        public float time;

    }

    public List<Keyframe> keyframes = new List<Keyframe>();

    public void AddKeyframe(FullBodyPose pose, float time)
    {
        Keyframe keyframe = new Keyframe()
        {
            pose = pose,
            time = time
        };
        keyframes.Add(keyframe);
    }

    public void SortKeyframes()
    {
        keyframes.Sort((a, b) => a.time.CompareTo(b.time));
    }

    public Keyframe GetPrevKeyframe(float time)
    {
        Keyframe prevKeyframe = null;
        // keyframes are assumed sorted
        foreach (var keyframe in keyframes)
        {
            if (keyframe.time > time)
            {
                break;
            }
            prevKeyframe = keyframe;
        }
        return prevKeyframe;
    }

    public Keyframe GetNextKeyframe(Keyframe keyframe)
    {
        int index = keyframes.IndexOf(keyframe);
        if (index < keyframes.Count - 1)
        {
            return keyframes[index + 1];
        }
        else
        {
            return keyframes[0];
        }
        return null;
    }


}