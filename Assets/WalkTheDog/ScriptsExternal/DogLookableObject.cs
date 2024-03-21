using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DogLookableObject : MonoBehaviour
{
    public static List<DogLookableObject> all = new List<DogLookableObject>();

    [Header("Place this marker where the dog should look.")]
    [Tooltip("Place this marker where the dog should look.")]
    public Transform lookPositionMarker;

    public Vector3 lookPosition
    {
        get
        {
            if (lookPositionMarker == null)
            {
                return transform.position;
            }
            return lookPositionMarker.position;
        }
    }

    private void Reset()
    {
        lookPositionMarker = transform;
    }

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
        Gizmos.DrawWireSphere(lookPosition, 0.6f);
        Gizmos.DrawWireSphere(lookPosition, 0.5f);

    }

}