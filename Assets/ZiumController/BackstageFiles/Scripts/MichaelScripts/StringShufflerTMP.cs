using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;
using TMPro; // Namespace for TextMesh Pro
public class VolumeInteractiveShuffle : MonoBehaviour
{
    public Transform playerTransform; // Assign your player's Transform here
    public TextMeshPro displayText; // Assign in the inspector
    public string[] stringsToDisplay; // Populate in the inspector
    private bool isPlayerInside = false; // To track if the player is inside the volume

    private void Start()
    {
        if (stringsToDisplay.Length > 0)
        {
            displayText.text = stringsToDisplay[0]; // Display the first string initially
        }
    }

    private void Update()
    {
        // Check if the player is inside the volume and presses 'E'
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            ShuffleStringsAndDisplay();
        }
    }

    private void ShuffleStringsAndDisplay()
    {
        if (stringsToDisplay.Length > 1)
        {
            Shuffle(stringsToDisplay);
            displayText.text = stringsToDisplay[0]; // Update display text with the first string in the shuffled array
        }
    }

    private void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + UnityEngine.Random.Range(0, n - i);
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
    }

    // OnTriggerEnter is called when the Collider other enters the trigger.
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == playerTransform) // Check if the collider belongs to the player
        {
            isPlayerInside = true;
        }
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger.
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == playerTransform) // Check if the collider belongs to the player
        {
            isPlayerInside = false;
        }
    }
}
