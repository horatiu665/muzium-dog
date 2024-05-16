using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToyBoxHHH;
using ToyBoxHHH.ReadOnlyUtil;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DogConcert : MonoBehaviour
{
    public Transform player => DogCastleReferences.instance.dogBrain.player;
    public DogConcertHideShow dogConcertHideShow;

    public PlayableDirector concertTimeline;

    public PlayableAsset concertMain;
    public PlayableAsset concertEnding;
    public PlayableAsset concertEncoreLoop;


    public bool useCheat = false;
    public KeyCode cheatKey = KeyCode.K;

    public ConcertCandleSystem candleSystem;


    [Header("Rules for approaching")]
    public Transform concertCenter;
    public float concertHearingThreshold = 100f;
    private bool playerIsInConcertRange = false;
    private float playerEnterTime = 0f;
    private float playerLeaveTime = 0f;
    public float timeUntilStopMusicWhenLeaving = 15f;

    public UnityEvent OnPlayerEnterConcertRadius_UE = new UnityEvent();
    public UnityEvent OnPlayerLeaveConcertRadius_UE = new UnityEvent();
    public event System.Action OnPlayerEnterConcertRadius;
    public event System.Action OnPlayerLeaveConcertRadius;

    [Header("hack replace timeline")]
    public TimelineAsset replaceWith;


    private void Update()
    {
        // cheat
        if (useCheat && Input.GetKeyDown(cheatKey))
        {
            RestartConcert();
        }

        // todo: maybe this should always run. worried right now about ending the concert and this doesn't run which might result in some issues with the settings applied within.
        if (dogConcertHideShow.concertState == DogConcertHideShow.ConcertState.Playing)
        {
            Update_ConcertDistance();
        }

    }


    private void Update_ConcertDistance()
    {
        // check if player is close enough to hear the concert
        var playerPos = player.position;

        // TODO: consider up/down distance 
        var dist = Vector3.Distance(playerPos, concertCenter.position);

        if (!playerIsInConcertRange && dist < concertHearingThreshold)
        {
            playerIsInConcertRange = true;
            playerEnterTime = Time.time;

            OnPlayerEnterConcertRadius?.Invoke();
            OnPlayerEnterConcertRadius_UE.Invoke();
        }
        if (playerIsInConcertRange && dist > concertHearingThreshold + 1)
        {
            playerIsInConcertRange = false;
            playerLeaveTime = Time.time;

            OnPlayerLeaveConcertRadius?.Invoke();
            OnPlayerLeaveConcertRadius_UE.Invoke();

        }

        // pause and resume the concert based on player's presence
        if (playerIsInConcertRange)
        {
            ContinueConcertSmart();

        }
        else if (!playerIsInConcertRange)
        {
            if (IsTimelinePlaying())
            {
                var timeSincePlayerLeft = Time.time - playerLeaveTime;
                if (timeSincePlayerLeft > timeUntilStopMusicWhenLeaving)
                {
                    PauseConcert();
                }
            }
        }
    }

    public void SetConcert(PlayableAsset whichOne)
    {
        concertTimeline.playableAsset = whichOne;
        RestartConcert();

    }

    [DebugButton]
    public void SetMainConcert()
    {
        SetConcert(concertMain);
    }

    [DebugButton]
    public void SetEndingConcert()
    {
        SetConcert(concertEnding);
    }

    [DebugButton]
    public void SetEncoreConcert()
    {
        SetConcert(concertEncoreLoop);
    }

    // this resumes the concert based on some rules
    // - if the player has never been to the concert, starts at the beginning
    // - if the player left and came back, continue from where it left plus a bit of buffer.
    // - if the candles are blown, play the end of the concert and then stop.
    // - do the gameplay of restarting the concert via the concert banner.... 
    // figure out the concert states maybe and let the banner set those states rather than directly playing/stopping the concert.
    public void ContinueConcertSmart()
    {
        // todo: implement the rules
        if (!IsTimelinePlaying())
        {
            ResumeConcert();
        }
    }


    private bool IsTimelinePlaying()
    {
        if (concertTimeline.playableGraph.IsValid() == false)
            return false;

        return concertTimeline.playableGraph.GetRootPlayable(0).GetSpeed() > 0;
    }

    [DebugButton]
    public void RestartConcert()
    {
        concertTimeline.time = 0;
        concertTimeline.Play();
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    [DebugButton]
    public void PauseConcert()
    {
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    [DebugButton]
    public void ResumeConcert()
    {
        if (concertTimeline.playableGraph.IsValid() == false)
        {
            RestartConcert();
            return;
        }

        concertTimeline.time = concertTimeline.time;
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    [DebugButton]
    public void ContinueConcertFromTime(float time)
    {
        concertTimeline.time = time;
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }


    [DebugButton]
    public void ReplaceTimeline()
    {
        var prevTimeline = concertTimeline.playableAsset as TimelineAsset;
        // save bindings in a list

        Dictionary<Object, Object> trackSourcesToBindings = new Dictionary<Object, Object>();

        // the old timeline is: 
        var curTimeline = concertTimeline.playableAsset;

        // outputs from the old timeline
        var savedOutputs = curTimeline.outputs.ToList();

        // now we should assign the new timeline
        concertTimeline.playableAsset = replaceWith;

        var newTimelineOutputs = concertTimeline.playableAsset.outputs.ToList();
        // set outputs of the new timeline
        for (int i = 0; i < newTimelineOutputs.Count; i++)
        {
            if (!newTimelineOutputs[i].sourceObject) continue;
            if (savedOutputs.Count < i)
                continue;

            concertTimeline.SetGenericBinding(
                newTimelineOutputs[i].sourceObject,
                concertTimeline.GetGenericBinding(savedOutputs[i].sourceObject));
        }




#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(concertTimeline);
#endif
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (playerIsInConcertRange)
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(concertCenter.position, concertHearingThreshold);
    }
}
