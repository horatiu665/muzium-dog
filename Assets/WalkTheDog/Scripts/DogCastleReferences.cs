using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
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
    public DogControlPanel dogControlPanel;

    public PlayableDirector dogConcertTimeline;
    public DogConcert dogConcert;

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




    /// <summary>
    /// Since I made so many debug settings that I tweak for the sake of testing the game, 
    /// we can use this button to reset all settings to their default values.
    /// this can also be used to see which settings are changed, by just reading the code...
    /// </summary>
    [DebugButton]
    public void Editor_SetToDefaultSettings()
    {
#if UNITY_EDITOR
        UnityEditor.Undo.IncrementCurrentGroup();
        var theUndoGroup = UnityEditor.Undo.GetCurrentGroup();
        UnityEditor.Undo.RegisterFullObjectHierarchyUndo(this, "Set to default settings");

        dogControlPanel.skipIntro = false;
        dogConcert.cheatSkipSecondsOnKey = false;




        UnityEditor.EditorUtility.SetDirty(dogControlPanel);
        UnityEditor.EditorUtility.SetDirty(dogConcert);

        UnityEditor.Undo.CollapseUndoOperations(theUndoGroup);
#endif
    }


}
