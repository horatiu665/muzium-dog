using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("Door")]
    public Transform doorRoot;

    public float angleFocus;
    public float angleOpen;
    public float angleClosed;

    private float targetAngle;

    public Transform knob;


    // public override void OnFocus()
    // {
    // }

    // public override void OnInteract()
    // {
    //     // isOpen = !isOpen;
    // }

    // public override void OnLoseFocus()
    // {
    // }

    private void Update()
    {
        var isOpen = false;
        var isFocused = false;

        if (DogControlPanel.instance.dogEnabled)
        {
            isOpen = true;
            isFocused = false;
        }

        targetAngle = isOpen ? angleOpen : isFocused ? angleFocus : angleClosed;
        knob.localRotation = Quaternion.Slerp(
            knob.localRotation,
            Quaternion.Euler(0, 0, isFocused ? -45 : 0),
            0.1f
        );
        doorRoot.rotation = Quaternion.Slerp(doorRoot.rotation,
            Quaternion.Euler(0, targetAngle, 0),
            Mathf.Clamp(Time.deltaTime * 15, 0, 1));

    }
}
