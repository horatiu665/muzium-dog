using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mute : MonoBehaviour
{

    public AudioListener audio;

    void Start()
    {
        //Reference to the AudioListener component on the object
        audio = GetComponent<AudioListener>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
             audio.enabled = !audio.enabled;
    }
}