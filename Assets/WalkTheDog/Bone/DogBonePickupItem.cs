using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBonePickupItem : ItemBehaviour
{
    private Vector3 initLocalPos;

    private Vector3 targetLocalPos;

    public Transform boneArt;
    public float boneArtLift = 0.1f;

    [TextArea(3, 10)]
    public string customText = "Oh! A Bone! It has an address written on it..."
                + "\n\n"
                + "It says: \"If found, please return to Dog Castle\""
                + "\n\n"
                + "I better hold on to this.";

    public override void Awake()
    {
        base.Awake();

        initLocalPos = boneArt.localPosition;
        targetLocalPos = initLocalPos;

    }

    void Update()
    {
        boneArt.localPosition = Vector3.Lerp(boneArt.localPosition, targetLocalPos, Time.deltaTime * 10);
    }

    public override void OnFocus()
    {
        base.OnFocus();

        targetLocalPos = initLocalPos + Vector3.up * boneArtLift;
    }

    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log(Time.time + " - DogBonePickupItem.OnInteract()");

        // show poem with text about this bone

        PoemSystem.instance.ShowCustomText(customText, true);

        this.ThrowItem(Vector3.forward, 1);

        this.gameObject.SetActive(false);

        // sound?

    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();

        targetLocalPos = initLocalPos;

    }

}
