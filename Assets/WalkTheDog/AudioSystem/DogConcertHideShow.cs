using System.Collections.Generic;
using ToyBoxHHH;
using ToyBoxHHH.ReadOnlyUtil;
using UnityEngine;

public class DogConcertHideShow : MonoBehaviour
{
    public DogConcert dogConcert;



    [Header("Concert state")]
    [ReadOnly]
    public ConcertState concertState = ConcertState.Hidden;
    public ConcertState initConcertState = ConcertState.Hidden;

    public enum ConcertState
    {
        // stage upside down, piano hidden, audience scattered
        Hidden,

        // stage rotates up, audience arrives.
        Starting,

        // stage is up, dog is playing, audience is watching. concert pauses when player out of range
        Playing,

        // stage rotates down, audience scatters.
        Ending,
    }

    [System.Serializable]
    public class ConcertHidingData
    {
        public Transform stageToRotate;

        public Transform stageRotationVisible, stageRotationHidden;
        [HideInInspector]
        public Quaternion stageTargetRotation;

        public float stageRotationSmoothness = 0.1f;


    }
    public ConcertHidingData ch;

    public List<DogConcertAudience> audience = new();

    public bool concertEnding_hidePiano = false;
    public bool concertEnding_byebye = false;


    private void OnEnable()
    {
        SetConcertState(initConcertState);
        // audience rotate on spot...
        foreach (var a in audience)
        {
            a.RotateOnSpot();
        }
    }

    private void Update()
    {
        if (concertEnding_hidePiano && concertState == ConcertState.Playing)
        {
            SetConcertState(ConcertState.Ending);
        }
        else if (concertEnding_byebye && concertState == ConcertState.Ending)
        {
            SetConcertState(ConcertState.Hidden);
        }

        Update_ConcertExists();
    }


    private void Update_ConcertExists()
    {
        // rotate stage to its target rotation.
        ch.stageToRotate.rotation = Quaternion.Slerp(
            ch.stageToRotate.rotation, ch.stageTargetRotation, ch.stageRotationSmoothness * Time.deltaTime * 60f);

    }

    [DebugButton]
    public void Editor_SetNextState()
    {
        SetConcertState((ConcertState)((int)(concertState + 1) % 4));
    }

    public void SetAudience(bool isAtConcert)
    {
        foreach (var a in audience)
        {
            a.isAtConcert = isAtConcert;
        }
    }

    public CastleRoadblock roadblock;

    public void SetConcertState(ConcertState newState)
    {
        concertState = newState;

        if (newState == ConcertState.Hidden)
        {
            ch.stageTargetRotation = ch.stageRotationHidden.rotation;
            SetAudience(false);

        }
        else if (newState == ConcertState.Starting)
        {
            ch.stageTargetRotation = ch.stageRotationVisible.rotation;
            SetAudience(true);
            
        }
        else if (newState == ConcertState.Playing)
        {
            ch.stageTargetRotation = ch.stageRotationVisible.rotation;
            SetAudience(true);
            
        }
        else if (newState == ConcertState.Ending)
        {
            ch.stageTargetRotation = ch.stageRotationHidden.rotation;
            SetAudience(true);
        }

        // refresh bridge thing.
        roadblock.RefreshRoadblockState();

    }



}