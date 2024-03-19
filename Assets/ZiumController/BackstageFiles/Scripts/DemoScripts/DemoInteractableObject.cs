using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoInteractableObject : ItemBehaviour
{

    [SerializeField] private Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
    private Vector3 initialScale;

    public override void Awake()
    {
        base.Awake();
        initialScale = transform.localScale;
    }

    public override void OnFocus()
    {
        transform.localScale = hoverScale;
        Debug.Log("OnFocus");
    }

    public override void OnInteract()
    {
        Debug.Log("OnInteract");
        GetComponent<MeshRenderer>().materials[0].color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    public override void OnLoseFocus()
    {
        Debug.Log("OnLoseFocus");
        transform.localScale = initialScale;
    }
}
