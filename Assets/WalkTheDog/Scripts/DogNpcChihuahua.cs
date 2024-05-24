using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToyBoxHHH;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Events;

public class DogNpcChihuahua : MonoBehaviour
{
    [Header("Activates when dog enters proximity")]
    public DogProximity dogProximity;

    public DogSniffableObject dogSniffableObject;

    public Transform mainDogTransform => DogCastleReferences.instance.dog.transform;
    public Transform playerTransform => DogCastleReferences.instance.dogBrain.player;

    public Transform toScale;
    private float targetScale = 0f;

    public List<DogBarkAnim> dogGrowls = new();

    public List<DogBarkAnim> dogBarks = new();

    public List<DogBarkAnim> dogPants = new();

    public bool isFriendly = false;
    private float isFriendlyTime = 0f;
    public float stayFriendlyTime = 60;

    public int sniffsUntilFriendly = 3;
    private int numSniffs = 0;

    // freezes rotation for a while so it can be sniffed.
    public float freezeRotationDuration = 7f;
    private float freezeRotationTimer = 0f;
    private bool isFrozen;

    public Transform lookTarget;

    [DebugButton]
    public void GetRefsFromChildren()
    {
        if (dogProximity == null)
            dogProximity = GetComponent<DogProximity>();

        dogGrowls = GetComponentsInChildren<DogBarkAnim>(true).Where(db => db.name.ToLower().Contains("growl")).ToList();
        dogBarks = GetComponentsInChildren<DogBarkAnim>(true).Where(db => db.name.ToLower().Contains("bark")).ToList();
        dogPants = GetComponentsInChildren<DogBarkAnim>(true).Where(db => db.name.ToLower().Contains("pant")).ToList();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }


    private void OnEnable()
    {
        // disable all barks and stuff
        foreach (var dogBark in dogGrowls)
        {
            dogBark.activated = false;
        }
        foreach (var dogBark in dogBarks)
        {
            dogBark.activated = false;
        }
        foreach (var dogBark in dogPants)
        {
            dogBark.activated = false;
        }

        dogProximity.OnDogAreaChanged += OnDogAreaChanged;
        dogSniffableObject.OnSniffed += OnSniffed;

    }

    private void OnDisable()
    {
        dogProximity.OnDogAreaChanged -= OnDogAreaChanged;
        dogSniffableObject.OnSniffed -= OnSniffed;
    }

    private void OnDogAreaChanged(DogProximity.AreaData newArea, DogProximity.AreaData oldArea)
    {
        bool shouldShow = false;
        bool shouldBark = false;
        bool shouldGrowl = false;
        bool shouldPant = false;

        if (isFriendly && (Time.time - isFriendlyTime > stayFriendlyTime))
        {
            BecomeUnfriendly();
        }

        if (newArea == null)
        {
            // dog left. hide.
            lookTarget = null;
        }
        else if (newArea.areaName == "CLOSE")
        {
            // do smth when dog is close
            shouldShow = true;

            lookTarget = null;

            if (isFriendly)
            {
                shouldPant = true;

                // look at player or main dog
                lookTarget = Random.value > 0.66f ? mainDogTransform : Random.value > 0.33f ? playerTransform : null;
            }
            else
            {
                lookTarget = mainDogTransform;
                shouldBark = true;

            }

            //BecomeFriendly();

        }
        else if (newArea.areaName == "MID")
        {
            lookTarget = mainDogTransform;
            // if player is closer, look at player
            var distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            var distToDog = Vector3.Distance(transform.position, mainDogTransform.position);
            if (distToPlayer < distToDog)
                lookTarget = playerTransform;

            // do smth when dog is mid
            shouldShow = true;

            if (!isFriendly)
                shouldGrowl = true;



        }
        else if (newArea.areaName == "FAR")
        {
            lookTarget = mainDogTransform;

            // if player is closer, look at player
            var distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            var distToDog = Vector3.Distance(transform.position, mainDogTransform.position);
            if (distToPlayer < distToDog)
                lookTarget = playerTransform;

            // do smth when dog is far
            shouldShow = true;
            
        }

        targetScale = shouldShow ? 1 : 0;


        foreach (var dogBark in dogBarks)
        {
            dogBark.activated = shouldBark;
        }

        foreach (var dogBark in dogPants)
        {
            dogBark.activated = shouldPant;
        }

        foreach (var dogBark in dogGrowls)
        {
            dogBark.activated = shouldGrowl;
        }


    }

    private void BecomeFriendly()
    {
        // becomes friendly when dog is close ONCE. 
        isFriendly = true;
        isFriendlyTime = Time.time;

    }

    private void BecomeUnfriendly()
    {
        isFriendly = false;
        numSniffs = 0;
    }

    private void OnSniffed()
    {
        numSniffs++;
        if (!isFriendly && (numSniffs >= sniffsUntilFriendly))
        {
            BecomeFriendly();
        }
    }

    private void Update()
    {
        toScale.transform.localScale = Vector3.Lerp(toScale.transform.localScale, Vector3.one * targetScale, Time.deltaTime * 5f);

        if (lookTarget != null)
        {
            freezeRotationTimer += Time.deltaTime;
            if (freezeRotationTimer > freezeRotationDuration)
            {
                isFrozen = !isFrozen;
                freezeRotationTimer = 0;
            }

            if (isFrozen)
            {
                return;
            }

            var lookPos = lookTarget.position;
            lookPos.y = toScale.transform.position.y;
            toScale.transform.LookAt(lookPos);

        }
        else
        {
            toScale.transform.localRotation = Quaternion.identity;
        }


    }


}
