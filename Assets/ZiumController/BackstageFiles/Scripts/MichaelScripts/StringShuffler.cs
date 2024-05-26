using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StringShuffler : MonoBehaviour
{
    public Text displayText; // Assign this in the inspector
    public Button shuffleButton; // Assign this in the inspector
    public string[] stringsToDisplay; // Populate this array in the inspector

    private void Start()
    {
        if (stringsToDisplay.Length > 0)
        {
            displayText.text = stringsToDisplay[0]; // Display the first string initially
        }

        shuffleButton.onClick.AddListener(ShuffleStringsAndDisplay); // Add listener for button click
    }

    private void ShuffleStringsAndDisplay()
    {
        if (stringsToDisplay.Length > 1)
        {
            Shuffle(stringsToDisplay); // Shuffle the array
            displayText.text = stringsToDisplay[0]; // Display the first string of the shuffled array
        }
    }

    private void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            // Unity's random range is inclusive of the first parameter and exclusive of the second for integers
            int r = i + UnityEngine.Random.Range(0, n - i);
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
    }
}
