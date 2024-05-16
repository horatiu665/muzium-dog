using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DogCastleReferences : MonoBehaviour
{
    private static DogCastleReferences _instance;
    public static DogCastleReferences instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DogCastleReferences>();
            }
            return _instance;
        }
    }
    
    [Header("Timelines")]
    public TimelineAsset dogIntro;

    public TimelineAsset dogLeaveCastleReset;


    [Header("Important objects")]
    public GameObject dog;

    public DogBrain dogBrain;

    public PlayableDirector dogConcertTimeline;

    [Tooltip("The gate where you get the dog. a.k.a. the Dog Control Panel")]
    public GameObject dogGate;

    public GameObject dogElevator;

    [Tooltip("The hand's parent - lives on the main camera. the hand itself, handVisual, might be reparented based on its state")]

    public GameObject knockerHandUnderCamera;




    [Header("Useful prefabs")]
    public GameObject chihuahua;
    public GameObject firedog;
    public GameObject doggyman;

    public GameObject plantGeneric;



}
