using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DogBarkableObject : MonoBehaviour
{
    public static List<DogBarkableObject> all = new List<DogBarkableObject>();

    [Header("The dog will look towards and bark at this marker if in range.")]
    [Tooltip("The dog will look towards and bark at this marker if in range.")]
    public Transform barkPositionMarker;

    public Vector3 barkPosition
    {
        get
        {
            if (barkPositionMarker == null)
            {
                return transform.position;
            }
            return barkPositionMarker.position;
        }
    }

    private void Reset()
    {
        barkPositionMarker = transform;
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
        Gizmos.DrawWireSphere(barkPosition, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(barkPosition + new Vector3(0.5f, 0.5f, 0), barkPosition + new Vector3(-0.5f, -0.5f, 0));
        Gizmos.DrawLine(barkPosition + new Vector3(-0.5f, 0.5f, 0), barkPosition + new Vector3(0.5f, -0.5f, 0));


    }

}