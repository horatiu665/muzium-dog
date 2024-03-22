using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DogSniffableObject : MonoBehaviour
{
    public static List<DogSniffableObject> all = new List<DogSniffableObject>();

    [Header("The dog will walk to and sniff this object, and face towards the transform this script is on.")]
    [Tooltip("The dog will walk to and sniff this object, and face towards the transform this script is on.")]
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

    private void Reset()
    {
        sniffPositionMarker = transform;
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
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, sniffPosition);

    }

}