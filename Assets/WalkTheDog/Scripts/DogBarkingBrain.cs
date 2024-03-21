using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogBarkingBrain : MonoBehaviour
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

    public float barkableRadius = 10;

    private List<DogBarkableObject> barkablesWithinRange = new List<DogBarkableObject>();

    public float updateRate = 0.5f;
    private float lastUpdate = 0;

    public DogBarkableObject dogBarkableObjectTarget;

    public bool AnyBarkables()
    {
        return barkablesWithinRange.Count > 0;
    }
    public List<DogBarkableObject> GetBarkables()
    {
        return barkablesWithinRange;
    }
    public DogBarkableObject GetRandom()
    {
        if (barkablesWithinRange.Count == 0)
        {
            return null;
        }
        return barkablesWithinRange[Random.Range(0, barkablesWithinRange.Count)];
    }


    private void Update()
    {
        if (Time.time - lastUpdate > updateRate)
        {
            lastUpdate = Time.time;
            ComputeBarkablesInRange();

        }

        if (dogBarkableObjectTarget != null)
        {
            // do sniff animation towards that object
            dogRefs.dogLocomotion.SetTargetRotation(dogBarkableObjectTarget.barkPosition - transform.position);
            dogRefs.dogBrain.dogLook.LookAtPosition(dogBarkableObjectTarget.barkPosition);
        }
        else
        {
            dogRefs.dogBrain.dogLook.LookAt(null);
        }

    }

    public void Bark(){
        //???
    }

    private void ComputeBarkablesInRange()
    {
        barkablesWithinRange.Clear();
        foreach (var b in DogBarkableObject.all)
        {
            if (Vector3.Distance(transform.position, b.transform.position) < barkableRadius)
            {
                barkablesWithinRange.Add(b);
            }
        }
    }
}