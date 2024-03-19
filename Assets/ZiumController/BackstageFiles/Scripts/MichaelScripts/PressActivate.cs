using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressActivate : MonoBehaviour

   
{
    public GameObject gameobject;
    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            gameobject.SetActive(true);
        }
    }
}