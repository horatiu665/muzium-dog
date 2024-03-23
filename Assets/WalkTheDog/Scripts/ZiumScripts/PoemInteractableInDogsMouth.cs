using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoemInteractableInDogsMouth : ItemBehaviour
{
    public event System.Action OnInteractedEvent;

    private Transform oldParentOnDogMouth;

    public override void Awake()
    {
        base.Awake();
        
        // obj parent must be null.
        oldParentOnDogMouth = transform.parent;
        transform.SetParent(null);
    }

    private void Update()
    {
        transform.position = oldParentOnDogMouth.position;
        transform.rotation = oldParentOnDogMouth.rotation;

    }

    public override void OnFocus()
    {

    }

    public override void OnInteract()
    {

        OnInteractedEvent?.Invoke();

    }

    public override void OnLoseFocus()
    {


    }
}
