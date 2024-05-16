using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBonePickupItem : ItemBehaviour
{
    private Vector3 initLocalPos;

    private Vector3 targetLocalPos;

    public override void Awake()
    {
        base.Awake();

        initLocalPos = transform.localPosition;
        targetLocalPos = initLocalPos;

    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * 10);
    }

    public override void OnFocus()
    {
        base.OnFocus();

        targetLocalPos = initLocalPos + Vector3.up * 0.2f;
    }

    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log(Time.time + " - DogBonePickupItem.OnInteract()");

        // show poem with text about this bone

        PoemSystem.instance.ShowCustomText("Oh! A Bone! It has an address written on it..."
            + "\n\n"
            + "It says: \"If found, please return to Dog Castle\""
            + "\n\n"
            + "I better hold on to this."
        , true);

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
