using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using TMPro;
using System.Collections;

public class CubeRotator : MonoBehaviour
{
    public Transform cubeTransform;
    public TextMeshPro textDisplay;
    public string[] texts = new string[4];
    public int currentRotationIndex = 0;
    private bool isPlayerInside = false;
    public float rotationDuration = 1.0f; // Duration of the rotation in seconds

    void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(RotateCubeOverTime(Vector3.forward * 90f)); // Rotate around Z-axis
        }
    }

    IEnumerator RotateCubeOverTime(Vector3 byAngles)
    {
        var fromAngle = cubeTransform.rotation;
        var toAngle = Quaternion.Euler(cubeTransform.eulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / rotationDuration)
        {
            cubeTransform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
        cubeTransform.rotation = toAngle; // Ensure the target rotation is precisely reached
        currentRotationIndex = (currentRotationIndex + 1) % 4;
        textDisplay.text = texts[currentRotationIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}
