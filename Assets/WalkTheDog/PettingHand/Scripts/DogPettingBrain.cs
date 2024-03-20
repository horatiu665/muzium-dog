using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogPettingBrain : MonoBehaviour
{

    [SerializeField]
    private DogRefs _dogRefs;
    public DogRefs dogRefs
    {
        get
        {
            if (_dogRefs == null)
            {
                _dogRefs = GetComponentInParent<DogRefs>();
            }
            return _dogRefs;
        }
    }

    public float pettingNeed = 0f;
    public Vector2 pettingNeedRange = new Vector2(-1, 1f);

    [Tooltip("How much petting need increases per second")]
    public float pettingNeedIncrease = 0.1f;
    public float pettingNeedReducedWhenPetted = 0.2f;

    // keeps track of the objects that are isDog=true and are currently being petted
    private HashSet<PettableObject> curPettedDogObjects = new HashSet<PettableObject>();

    void OnEnable()
    {
        DogPettingHand.instance.OnPettingStart += OnPettingStart;
        DogPettingHand.instance.OnPettingEnd += OnPettingEnd;
    }

    private void OnDisable()
    {
        DogPettingHand.instance.OnPettingStart -= OnPettingStart;
        DogPettingHand.instance.OnPettingEnd -= OnPettingEnd;
    }

    private void OnPettingStart(PettableObject pettableObject)
    {
        if (pettableObject.isDog)
        {
            curPettedDogObjects.Add(pettableObject);
        }
    }

    private void OnPettingEnd(PettableObject pettableObject)
    {
        if (pettableObject.isDog)
        {
            curPettedDogObjects.Remove(pettableObject);
        }
    }

    public bool IsBeingPetted()
    {
        return curPettedDogObjects.Count > 0;

    }

    void Update()
    {
        if (IsBeingPetted())
        {
            pettingNeed -= Time.deltaTime * pettingNeedReducedWhenPetted;
        }
        else
        {
            pettingNeed += Time.deltaTime * pettingNeedIncrease;
        }
        pettingNeed = Mathf.Clamp(pettingNeed, pettingNeedRange.x, pettingNeedRange.y);

    }
}