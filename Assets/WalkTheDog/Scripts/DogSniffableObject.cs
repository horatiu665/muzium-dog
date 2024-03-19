using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DogSniffableObject : MonoBehaviour
{

    public static List<DogSniffableObject> allSniffableObjects = new List<DogSniffableObject>();

    [Header("Place this marker where the dog should sniff.")]
    public Transform sniffPositionMarker;

    public Vector3 sniffPosition
    {
        get
        {
            if (sniffPositionMarker == null)
            {
                return transform.position;
            }
            return sniffPositionMarker.position;
        }
    }

    private void OnEnable()
    {
        allSniffableObjects.Add(this);
    }

    private void OnDisable()
    {
        allSniffableObjects.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, sniffPosition);

    }

}