using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ItemBehaviour
{
    public override void OnInteract()
    {
        Debug.Log("caca");
    }

    public override void OnFocus()
    {
        Debug.Log("cacaca");
    }

    public override void OnLoseFocus()
    {
        Debug.Log("cacacaca");
    }
}
