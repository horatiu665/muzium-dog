using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToyBoxHHH;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Events;


public class DogNpcDoggyman : MonoBehaviour
{
    [Header("Activates when dog enters proximity")]
    public DogProximity dogProximity;

    private DogSniffableObject dogSniffableObject;

    public Transform mainDogTransform => DogCastleReferences.instance.dog.transform;
    public Transform playerTransform => DogCastleReferences.instance.dogBrain.player;

    public Transform toScale;
    private float curScale = 0f;
    private float targetScale = 0f;

    // activate these so the main dog can bark at this dog.
    public List<DogBarkableObject> dogBarkableObjects = new();

    public DogBarkAnim growl;
    public DogBarkAnim talking;

    public bool isFriendly = false;
    private float isFriendlyTime = 0f;
    public float stayFriendlyTime = 60;

    public PettableObject pettableObject;
    public bool sayRandomThingWhenPetted = true;

    public int sniffsUntilFriendly = 3;
    private int numSniffs = 0;

    // freezes rotation for a while so it can be sniffed.
    public float freezeRotationDuration = 7f;
    private float freezeRotationTimer = 0f;
    private bool isFrozen;

    public Transform lookTarget;

    private bool shouldGrowl, shouldTalk;
    private bool isDogInsideCloseArea = false;

    public Vector2 randomDelayBeforeTalking = new Vector2(1, 5f);

    private float lastTalkTime = 0;
    public float minTimeBeforeTalkingRandomWhileDogIsInsideCloseArea = 5f;

    [DebugButton]
    public void GetRefsFromChildren()
    {
        if (dogProximity == null)
            dogProximity = GetComponent<DogProximity>();

        pettableObject = GetComponentInChildren<PettableObject>();

        growl = GetComponentsInChildren<DogBarkAnim>(true).Where(db => db.name.ToLower().Contains("growl")).FirstOrDefault();
        talking = GetComponentsInChildren<DogBarkAnim>(true).Where(db => db.name.ToLower().Contains("talk")).FirstOrDefault();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }


    private void OnEnable()
    {
        dogSniffableObject = GetComponentInChildren<DogSniffableObject>();

        growl.activated = false;
        growl.playRandomSounds = false;
        talking.activated = true;
        talking.playRandomSounds = false;
        isDogInsideCloseArea = false;

        dogProximity.OnDogAreaChanged += OnDogAreaChanged;
        dogSniffableObject.OnSniffed += OnSniffed;
        pettableObject.OnPettingEnd += OnPettingEnd;

    }

    private void OnDisable()
    {
        dogProximity.OnDogAreaChanged -= OnDogAreaChanged;
        dogSniffableObject.OnSniffed -= OnSniffed;
        pettableObject.OnPettingEnd -= OnPettingEnd;
    }

    private void OnPettingEnd()
    {
        if (sayRandomThingWhenPetted)
        {
            SaySomethingRandom();
        }
    }

    private void OnDogAreaChanged(DogProximity.AreaData newArea, DogProximity.AreaData oldArea)
    {
        bool shouldShow = false;

        shouldGrowl = false;
        shouldTalk = false;
        isDogInsideCloseArea = false;

        bool shouldBeBarkedAt = false;

        if (isFriendly && (Time.time - isFriendlyTime > stayFriendlyTime))
        {
            BecomeUnfriendly();
        }

        if (newArea == null)
        {
            // dog left. hide.
            lookTarget = null;
            shouldBeBarkedAt = false;
        }
        else if (newArea.areaName == "CLOSE")
        {
            // do smth when dog is close
            shouldShow = true;

            lookTarget = null;

            isDogInsideCloseArea = true;

            if (!isFriendly)
            {
                shouldBeBarkedAt = true;
            }

            // look at player or main dog
            RandomizeLookTarget();

            SaySomethingRandom();

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

            if (!isFriendly)
                shouldBeBarkedAt = true;

            SaySomethingRandom();

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

        foreach (var db in dogBarkableObjects)
        {
            db.enabled = shouldBeBarkedAt;
        }

    }

    private void RandomizeLookTarget()
    {
        // between dog and player
        lookTarget = Random.value > 0.551f ? mainDogTransform : playerTransform;
    }

    private void BecomeFriendly()
    {
        // becomes friendly when dog is close ONCE. 
        isFriendly = true;
        isFriendlyTime = Time.time;

        foreach (var db in dogBarkableObjects)
        {
            db.enabled = false;
        }

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

        SaySomethingRandom();
    }

    private void SaySomethingRandom()
    {
        lastTalkTime = Time.time;

        StartCoroutine(pTween.Wait(Random.Range(randomDelayBeforeTalking.x, randomDelayBeforeTalking.y), () =>
        {
            talking.smartSoundDog.Play();
            lastTalkTime = Time.time;
        }));
    }

    private void Update()
    {
        curScale = Mathf.Lerp(curScale, targetScale, Time.deltaTime * 5f);
        if (curScale < 0.01f)
        {
            toScale.gameObject.SetActive(false);
        }
        else
        {
            toScale.gameObject.SetActive(true);
            toScale.transform.localScale = Vector3.one * curScale;
        }
        // toScale.transform.localScale = Vector3.Lerp(toScale.transform.localScale, Vector3.one * targetScale, Time.deltaTime * 5f);

        if (lookTarget != null)
        {
            freezeRotationTimer += Time.deltaTime;
            if (freezeRotationTimer > freezeRotationDuration)
            {
                isFrozen = !isFrozen;
                freezeRotationTimer = 0;

                // randomize look target after each freeze
                RandomizeLookTarget();

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

        if (isDogInsideCloseArea)
        {
            if (Time.time - lastTalkTime > minTimeBeforeTalkingRandomWhileDogIsInsideCloseArea)
            {
                SaySomethingRandom();
            }
        }
    }


}
