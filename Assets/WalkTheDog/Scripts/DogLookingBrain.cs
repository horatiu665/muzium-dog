using System;
using System.Collections.Generic;
using System.Linq;
using ToyBoxHHH;
using UnityEngine;

public class DogLookingBrain : MonoBehaviour
{

    [Header("This script finds objects to look at")]
    public float lookingRadius = 10f;

    public float updateRate = 0.551f;
    private float nextUpdate;

    private List<DogLookableObject> lookablesCache = new();

    void Update()
    {
        if (Time.time > nextUpdate)
        {
            nextUpdate = Time.time + updateRate;
            lookablesCache.Clear();
            var lookablesInRange = DogLookableObject.all.Where(lo => (lo.transform.position - transform.position).magnitude < lookingRadius);
            lookablesCache.AddRange(lookablesInRange);

        }
    }

    public DogLookableObject GetObjectToLookAt()
    {
        return lookablesCache.Random();
    }

    public bool AnyLookables()
    {
        return lookablesCache.Any();
    }
}