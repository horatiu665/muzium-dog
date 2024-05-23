using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class DogEyeController2D : MonoBehaviour
{

    public Renderer eyeRenderer;

    public int eyeFrames = 10;
    public int curEyeFrame;

    private void Reset()
    {
        eyeRenderer = GetComponent<Renderer>();
    }

    [DebugButton]
    public void SetEye(int i)
    {
        curEyeFrame = i;
        // set offset 
        eyeRenderer.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(1f / eyeFrames * i, 0));
    }

    [DebugButton]
    public void SetNextEye()
    {
        curEyeFrame++;
        if (curEyeFrame >= eyeFrames)
        {
            curEyeFrame = 0;
        }
        SetEye(curEyeFrame);
    }

}
