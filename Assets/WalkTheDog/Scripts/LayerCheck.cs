using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerCheck : MonoBehaviour
{
    public string layerName = "DOG";

    void Start()
    {
        if (gameObject.layer != LayerMask.NameToLayer(layerName))
        {
            Debug.LogError("LayerCheck: " + gameObject.name + " is not on layer " + layerName);
        }
    }
    
}
