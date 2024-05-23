using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogEyes2D : MonoBehaviour
{
    // dog brain
    private DogBrain _dogBrain;
    public DogBrain dogBrain
    {
        get
        {
            if (_dogBrain == null)
            {
                _dogBrain = GetComponentInParent<DogBrain>();
            }
            return _dogBrain;
        }
    }

    public Transform dogHeadFwd;

    public DogEyeController2D leftEye;
    public DogEyeController2D rightEye;

    public int frameLeft, frameRight, frameForward, frameUp, frameDown;

    public int[] frameBlink = new int[] { 5, 6, 7 };
    private float[] angles = new float[] { 0, 0, 0, 0 };

    public void SetEye(int frame)
    {
        leftEye.SetEye(frame);
        rightEye.SetEye(frame);
    }

    // for testing
    // public Transform lookTarget;
    // void Update()
    // {
    //     var localDir = lookTarget.position - dogHeadFwd.position;
    //     localDir = dogHeadFwd.InverseTransformDirection(localDir);
    //     SetDirection(localDir);
    // }

    public void LookAt(Vector3 worldPos)
    {
        var localDir = worldPos - dogHeadFwd.position;
        localDir = dogHeadFwd.InverseTransformDirection(localDir);
        SetDirection(localDir);
    }

    public void SetDirection(Vector3 localDir)
    {
        localDir = localDir.normalized;

        // var dotColor = Color.blue;
        var dot = Vector3.Dot(localDir, Vector3.forward);
        if (dot > 0.5f)
        {
            SetEye(frameForward);

            // dotColor = Color.blue;
        }
        else if (dot < -0.5f)
        {
            SetEye(frameForward);
            // dotColor = Color.magenta;
            //SetEye(frameBlink[Random.Range(0, frameBlink.Length)]);
        }
        else
        {
            angles[0] = Vector3.Angle(localDir, Vector3.up);
            angles[1] = Vector3.Angle(localDir, Vector3.down);
            angles[2] = Vector3.Angle(localDir, Vector3.right);
            angles[3] = Vector3.Angle(localDir, Vector3.left);

            var minIndex = 0;
            for (int i = 1; i < 4; i++)
            {
                if (angles[i] <= angles[minIndex])
                {
                    minIndex = i;
                }
            }

            switch (minIndex)
            {
                case 0:
                    SetEye(frameUp);
                    // dotColor = Color.green;
                    break;
                case 1:
                    SetEye(frameDown);
                    // dotColor = Color.yellow;
                    break;
                case 2:
                    SetEye(frameRight);
                    // dotColor = Color.red;
                    break;
                case 3:
                    SetEye(frameLeft);
                    // dotColor = Color.cyan;
                    break;
            }

        }

        // Debug.DrawRay(dogHeadFwd.position, dogHeadFwd.TransformDirection(Vector3.up), Color.green);
        // Debug.DrawRay(dogHeadFwd.position, dogHeadFwd.TransformDirection(Vector3.down), Color.green);
        // Debug.DrawRay(dogHeadFwd.position, dogHeadFwd.TransformDirection(Vector3.right), Color.red);
        // Debug.DrawRay(dogHeadFwd.position, dogHeadFwd.TransformDirection(Vector3.left), Color.yellow);
        // Debug.DrawRay(dogHeadFwd.position, dogHeadFwd.TransformDirection(localDir), dotColor);



    }
}
