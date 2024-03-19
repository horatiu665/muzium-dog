using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressDeactivate : MonoBehaviour

   
{
    public GameObject gameobject;
    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            gameobject.SetActive(false);
        }
    }
}