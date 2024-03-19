using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class BreathingEffect : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float frequency = 1f;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        float newY = originalPosition.y + amplitude * Mathf.Sin(frequency * Time.time);
        transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
    }
}
