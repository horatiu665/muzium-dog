using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogSniffingBrain : MonoBehaviour
{

    [SerializeField]
    private DogRefs _dogRefs;
    public DogRefs dogRefs
    {
        get
        {
            if (_dogRefs == null)
            {
                _dogRefs = GetComponent<DogRefs>();
            }
            return _dogRefs;
        }
    }

    public float sniffableRadius = 10;

    private List<DogSniffableObject> sniffablesWithinRange = new List<DogSniffableObject>();

    public float sniffableUpdateRate = 0.5f;
    private float lastSniffableUpdate = 0;

    public float sniffDistance = 0.3f;

    public DogSniffableObject sniffAnimationTarget;

    public bool AnySniffables()
    {
        return sniffablesWithinRange.Count > 0;
    }
    public List<DogSniffableObject> GetSniffables()
    {
        return sniffablesWithinRange;
    }
    public DogSniffableObject GetRandomSniffable()
    {
        if (sniffablesWithinRange.Count == 0)
        {
            return null;
        }
        return sniffablesWithinRange[Random.Range(0, sniffablesWithinRange.Count)];
    }


    private void Update()
    {
        if (Time.time - lastSniffableUpdate > sniffableUpdateRate)
        {
            lastSniffableUpdate = Time.time;
            ComputeSniffablesInRange();

        }


        if (sniffAnimationTarget != null)
        {
            // do sniff animation towards that object
            dogRefs.dogLocomotion.SetTargetRotation(sniffAnimationTarget.transform.position - transform.position);
            dogRefs.anim.SetBool("Sniff", true);
        }
        else
        {
            dogRefs.anim.SetBool("Sniff", false);
        }

    }

    private void ComputeSniffablesInRange()
    {
        // check for sniffable objects within range.
        sniffablesWithinRange.Clear();
        foreach (var sniffable in DogSniffableObject.allSniffableObjects)
        {
            if (Vector3.Distance(transform.position, sniffable.transform.position) < sniffableRadius)
            {
                sniffablesWithinRange.Add(sniffable);
            }
        }
    }
}