using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DogLookableObject : MonoBehaviour
{
    public static List<DogLookableObject> all = new List<DogLookableObject>();

    [Header("Place this script on a transform the dog should look at.")]
    [Header("Optional: if set, the dog tries to go to this object before looking.")]
    public Transform optionalDogPosition;

    private void OnEnable()
    {
        all.Add(this);
    }

    private void OnDisable()
    {
        all.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        if (optionalDogPosition != null)
        {
            // draw horizontal circle on the ground
            for (int i = 0; i < 360; i+=10){
                var x = Mathf.Cos(i * Mathf.Deg2Rad) * 0.5f;
                var z = Mathf.Sin(i * Mathf.Deg2Rad) * 0.5f;
                var nextX = Mathf.Cos((i + 10) * Mathf.Deg2Rad) * 0.5f;
                var nextZ = Mathf.Sin((i + 10) * Mathf.Deg2Rad) * 0.5f;
                Gizmos.DrawLine(optionalDogPosition.position + new Vector3(x, 0, z), optionalDogPosition.position + new Vector3(nextX, 0, nextZ));
            }
            Gizmos.DrawLine(transform.position, optionalDogPosition.position);
        }


    }

}