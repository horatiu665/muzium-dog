using System;
using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudienceClap : MonoBehaviour
{
    public PianoPlayerCharacterController pianoPlayer;

    public bool shouldClap;
    private bool isClapping = false;

    public bool shouldAwoo;
    private bool isAwooing = false;


    // public bool shouldClapSynchronized = false;

    [Serializable]
    public class ClapGroups
    {
        public List<AudienceGroove> audienceMembers = new List<AudienceGroove>();

        public List<AudioClip> clapSounds = new();
        [HideInInspector]
        public List<AudioClip> clapReorderedCache = new();
        public float maxRandomDelay = 0.5f;

    }

    // instructions:
    // list of audience members in a group
    // has a list of clapping sounds
    // they are each assinged a random clap sound, no two members should clap the same sound

    public List<ClapGroups> clapGroups = new List<ClapGroups>();

    public ClapGroups awooGroup = new ClapGroups();

    [DebugButton]
    public void PutAudienceMembersIntoRandomClapGroups()
    {
        foreach (var cg in clapGroups)
        {
            cg.audienceMembers.Clear();
        }
        var am = FindObjectsOfType<AudienceGroove>(true);
        foreach (var ag in am)
        {
            clapGroups[Random.Range(0, clapGroups.Count)].audienceMembers.Add(ag);
        }

    }

    [DebugButton]
    public void PutAudienceMembersIntoAwooGroups(float chance01)
    {
        awooGroup.audienceMembers.Clear();
        var am = FindObjectsOfType<AudienceGroove>(true);
        foreach (var ag in am)
        {
            if (Random.value < chance01)
                awooGroup.audienceMembers.Add(ag);
        }
    }

    private void OnEnable()
    {
        if (pianoPlayer == null)
        {
            pianoPlayer = FindObjectOfType<PianoPlayerCharacterController>();
        }


    }

    void Update()
    {
        if (shouldClap && !isClapping)
        {
            // if (!shouldClapSynchronized)
            {
                isClapping = true;
                StartCoroutine(Clap());
            }
            // else
            // {
            //     isClapping = true;
            //     StartCoroutine(ClapSynchronized());
            // }

        }

        if (shouldAwoo && !isAwooing)
        {
            isAwooing = true;
            StartCoroutine(Awoo());

        }
    }
    IEnumerator ClapSynchronized()
    {
        // while (shouldClapSynchronized)
        {
            // play a single hit clap on all audience members with TINY delay.

            yield return 0;

        }

        isClapping = false;
    }

    IEnumerator Clap()
    {
        // start random clap sounds on the random audience
        // ensure clap sounds are not reused
        foreach (var cg in clapGroups)
        {
            cg.clapReorderedCache.Clear();
            cg.clapReorderedCache.AddRange(cg.clapSounds);
            cg.clapReorderedCache.Shuffle();
            for (int i = 0; i < cg.audienceMembers.Count; i++)
            {
                cg.audienceMembers[i].clapAudioSource.clip = cg.clapReorderedCache[i % cg.clapReorderedCache.Count];
                if (cg.audienceMembers[i].isActiveAndEnabled)
                    cg.audienceMembers[i].clapAudioSource.PlayDelayed(cg.maxRandomDelay * Random.value);
            }

        }

        // wait for the longest clap sound to finish
        float longestClap = 0;
        foreach (var cg in clapGroups)
        {
            foreach (var ag in cg.audienceMembers)
            {
                if (ag.clapAudioSource.clip.length > longestClap)
                {
                    longestClap = ag.clapAudioSource.clip.length;
                }
            }
        }
        yield return new WaitForSeconds(longestClap);

        isClapping = false;

    }

    IEnumerator Awoo()
    {
        awooGroup.clapReorderedCache.Clear();
        awooGroup.clapReorderedCache.AddRange(awooGroup.clapSounds);
        awooGroup.clapReorderedCache.Shuffle();
        for (int i = 0; i < awooGroup.audienceMembers.Count; i++)
        {
            awooGroup.audienceMembers[i].clapAudioSource.clip = awooGroup.clapReorderedCache[i % awooGroup.clapReorderedCache.Count];
            if (awooGroup.audienceMembers[i].isActiveAndEnabled)
                awooGroup.audienceMembers[i].clapAudioSource.PlayDelayed(awooGroup.maxRandomDelay * Random.value);
        }

        // wait for the longest sound to finish
        float longestAwoo = 0;
        foreach (var ag in awooGroup.audienceMembers)
        {
            if (ag.clapAudioSource.clip.length > longestAwoo)
            {
                longestAwoo = ag.clapAudioSource.clip.length;
            }
        }
        yield return new WaitForSeconds(longestAwoo);

        isAwooing = false;
    }




}

