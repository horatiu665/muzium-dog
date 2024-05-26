using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlantProximity : MonoBehaviour
{
    [Header("Reacts to when DOG enters proximity area")]
    public DogProximity dogProximity;

    public GameObject plantInnerArea;

    private float targetScale = 0f;
    private float curScale = 0f;

    private bool playerInside => dogProximity.areas[0].isPlayerInside;
    private bool dogInside => dogProximity.areas[0].isDogInside;

    public DogSniffableObject sniffableObject;

    private float sniffTimer;
    public AnimationCurve sniffScaleCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
    public AnimationCurve sniffRotZCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

    private bool beingSniffed = false;

    private void Awake()
    {
        if (sniffableObject == null)
        {
            sniffableObject = GetComponent<DogSniffableObject>();
        }
        curScale = plantInnerArea.transform.localScale.x;
    }

    private void OnEnable()
    {
        sniffableObject.OnSniffed += OnSniffed;
    }
    private void OnDisable()
    {
        sniffableObject.OnSniffed -= OnSniffed;
    }

    private void OnSniffed()
    {
        StopAllCoroutines();
        beingSniffed = true;
        // animate while being sniffed.
        StartCoroutine(pTween.To(1f, t =>
        {
            sniffTimer = t;

            curScale = Mathf.Lerp(curScale, sniffScaleCurve.Evaluate(t), Time.deltaTime * 5f);
            if (curScale < 0.01f)
            {
                plantInnerArea.SetActive(false);
            }
            else
            {
                plantInnerArea.SetActive(true);
                plantInnerArea.transform.localScale = Vector3.one * curScale;
            }
            plantInnerArea.transform.localEulerAngles = new Vector3(0, 0, sniffRotZCurve.Evaluate(sniffTimer));

            if (t == 1)
                beingSniffed = false;
        }));
    }

    private void Update()
    {
        UpdatePlantAnim();
    }


    private void UpdatePlantAnim()
    {
        if (!beingSniffed)
        {
            // keeping this update loop light.
            targetScale = (dogInside) ? 1f : 0f;
            curScale = Mathf.Lerp(curScale, targetScale, Time.deltaTime * 5f);
            if (curScale < 0.01f)
            {
                plantInnerArea.SetActive(false);
            }
            else
            {
                plantInnerArea.SetActive(true);
                plantInnerArea.transform.localScale = Vector3.one * curScale;
            }
            //plantInnerArea.transform.localScale = Vector3.Lerp(plantInnerArea.transform.localScale, Vector3.one * targetScale, Time.deltaTime * 5f);
        }

    }

}
