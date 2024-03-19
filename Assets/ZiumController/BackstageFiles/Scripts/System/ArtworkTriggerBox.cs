using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtworkTriggerBox : MonoBehaviour
{
    public bool unlocked;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Player"))
        {
            if (!unlocked) UserInterface.Instance.CatalogueUpdateMessage();
            unlocked = true;
        }
    }
}
