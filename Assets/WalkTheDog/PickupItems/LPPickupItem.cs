using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPPickupItem : ItemBehaviour
{
    private Vector3 initLocalPos;

    private Vector3 targetLocalPos;

    public Transform boneArt;
    public float boneArtLift = 0.1f;

    [TextArea(3, 10)]
    public string customText = "Hmm, the Mr. Woofmantic LP! Nice!";

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

        // targetLocalPos = initLocalPos;


        // this code is like the dog biscuit code: it hides the obj. to be replaced with zium code once we figure out how to make it work.
        {

            Debug.Log(Time.time + " - DogBonePickupItem.OnInteract()");

            // show poem with text about this bone

            PoemSystem.instance.ShowCustomText(customText, true);

            // this.ThrowItem(DogCastleReferences.instance.mainCamera.transform, 1);

            this.gameObject.SetActive(false);

            // sound?

        }

    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();

        targetLocalPos = initLocalPos;

    }

}
