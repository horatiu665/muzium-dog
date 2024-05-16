using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantProximity : MonoBehaviour
{
    public DogProximity dogProximity;

    public GameObject plantInnerArea;

    private float targetScale = 0f;

    private bool playerInside => dogProximity.areas[0].isPlayerInside;
    private bool dogInside => dogProximity.areas[0].isDogInside;


    private void Update()
    {
        UpdatePlantScale();
    }


    private void UpdatePlantScale()
    {
        targetScale = (playerInside || dogInside) ? 1f : 0f;
        plantInnerArea.transform.localScale = Vector3.Lerp(plantInnerArea.transform.localScale, Vector3.one * targetScale, Time.deltaTime * 5f);
    }

}
