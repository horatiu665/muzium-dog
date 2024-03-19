using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class CameraPositionsToggler : MonoBehaviour
{
    public Transform[] pos;
    public int currentPos = 0;

    private void OnEnable()
    {
        DoItNow();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            DoItNow();
        }
    }

    [DebugButton]
    void DoItNow()
    {
        currentPos++;
        if (currentPos >= pos.Length)
        {
            currentPos = 0;
        }
        transform.position = pos[currentPos].position;
        transform.rotation = pos[currentPos].rotation;
        transform.SetParent(pos[currentPos]);

    }
}
