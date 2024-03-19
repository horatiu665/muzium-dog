using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCycle : MonoBehaviour {
    
    // put this script on your camera
    // it's great for a Canvas for your UI
    
    private Camera cam;
    public float cycleSeconds = 10f; // set to say 0.5f to test
    
    void Awake() {
        
        cam = GetComponent<Camera>();
    }
    
    void Update() {
        
        cam.backgroundColor = Color.HSVToRGB(
        Mathf.Repeat(Time.time / cycleSeconds, 1f),
        0.2f,     // set to a pleasing value. 0f to 1f
        0.9f      // set to a pleasing value. 0f to 1f
        );
    }
    }
