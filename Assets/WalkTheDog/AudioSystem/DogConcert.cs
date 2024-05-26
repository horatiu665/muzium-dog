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

    private List<PlayableAsset> playedTimelines = new();

    public bool cheatSkipSecondsOnKey = false;
    public KeyCode cheatSkipKey = KeyCode.L;
    public float cheatSkipSeconds = 10f;
    public float cheatSkipSecondsMax = 60f;
    private float cheatSkipSecondsLastTime;
    private float cheatSkipSecondsCurValue;

    private bool isReplayingConcert = false;

    private void Start()
    {
        // haven't seen the concert when you start the game.
        // candleSystem.SetDogConcertSeenStatus(false);

    }

    private void Update()
    {
        // cheat
        if (useCheat && Input.GetKeyDown(cheatKey))
        {
            dogConcertHideShow.Editor_SetNextState();
        }

        // todo: maybe this should always run. worried right now about ending the concert and this doesn't run which might result in some issues with the settings applied within.
        if (dogConcertHideShow.concertState == DogConcertHideShow.ConcertState.Playing)
        {
            Update_ConcertDistance();
        }
        else
        {
            // if the concert is not playing, we should not be in the concert range.
            playerIsInConcertRange = false;

            // if concert is not playing we stop replaying the concert.
            isReplayingConcert = false;

        }

        if (cheatSkipSecondsOnKey && Input.GetKeyDown(cheatSkipKey))
        {
            if (Time.time - cheatSkipSecondsLastTime < 0.2f)
            {
                cheatSkipSecondsCurValue = Mathf.Lerp(cheatSkipSecondsCurValue, cheatSkipSecondsMax, 0.1f);
            }
            else
            {
                cheatSkipSecondsCurValue = cheatSkipSeconds;
            }
            cheatSkipSecondsLastTime = Time.time;
            ContinueConcertFromTime((float)(concertTimeline.time + cheatSkipSecondsCurValue));
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

            if (!isReplayingConcert)
            {
                // player just entered concert range
                if (candleSystem.AreAllArtworksCompleted())
                {
                    SetEndingConcert();
                }
            }

        }
        if (playerIsInConcertRange && dist > concertHearingThreshold + 1)
        {
            playerIsInConcertRange = false;
            playerLeaveTime = Time.time;

            OnPlayerLeaveConcertRadius?.Invoke();
            OnPlayerLeaveConcertRadius_UE.Invoke();

            // when you leave the concert area, after having been inside it, you have officially seen the concert
            // (so if it's the last artwork you see, this is the way to trigger the proper end of it....???)
            // candleSystem.SetDogConcertSeenStatus(true);


        }

        // pause and resume the concert based on player's presence
        if (playerIsInConcertRange)
        {
            // resume concert if it's not playing
            // the timelines themselves handle what happens at the end of them.
            if (!IsTimelinePlaying())
            {
                ResumeConcert();
            }
        }
        else if (!playerIsInConcertRange)
        {
            // if you leave the area, you are no longer replaying the concert.
            isReplayingConcert = false;

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

    private void SetConcert(PlayableAsset whichOne)
    {
        // we actually don't want to loop using the concert timeline, we want the script to control it.
        concertTimeline.extrapolationMode = DirectorWrapMode.None;

        concertTimeline.playableAsset = whichOne;
        RestartConcert();

    }

    public void RestartMainConcertFromTicketOffice()
    {
        Debug.Log("RestartMainConcertFromTicketOffice ");

        isReplayingConcert = true;

        SetConcert(concertMain);
        playedTimelines.Clear();

        // candleSystem.SetDogConcertSeenStatus(false);

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

    // Called by Ending timeline with an object. easier to setup visually in the timeline, altho confusing in the code.
    public void SetConcertStateToEndingAndHidden()
    {
        Debug.Log("SetConcertStateToEndingAndHidden ");

        dogConcertHideShow.SetConcertState(DogConcertHideShow.ConcertState.Ending);

        // set the concert to hidden after the ending is done.
        StartCoroutine(pTween.Wait(5, () =>
        {
            dogConcertHideShow.SetConcertState(DogConcertHideShow.ConcertState.Hidden);

        }));

    }

    // Called by Main concert timeline with an object. easier to setup visually in the timeline, altho confusing in the code.
    public void SetConcertToEncore()
    {
        Debug.Log("SetConcertToEncore ");

        SetEncoreConcert();
    }

    // Called by the timeline. If we finished all the artworks, we should head to the ending. Else we should loop the encore.
    public void CheckEncoreOrEnding()
    {
        Debug.Log("CheckEncoreOrEnding ");

        if (candleSystem.AreAllArtworksCompleted())
        {
            SetEndingConcert();
        }
        else
        {
            SetEncoreConcert();
        }
    }

    private bool IsTimelinePlaying()
    {
        if (!concertTimeline.playableGraph.IsValid())
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
        if (!concertTimeline.playableGraph.IsValid())
            return;

        concertTimeline.time = concertTimeline.time;
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    [DebugButton]
    public void ResumeConcert()
    {
        if (!concertTimeline.playableGraph.IsValid())
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
        if (!concertTimeline.playableGraph.IsValid())
            return;

        // clamp time
        if (time > (float)(concertTimeline.duration) - 1f)
        {
            time = (float)(concertTimeline.duration) - 1f;
        }

        concertTimeline.time = time;
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }


    [DebugButton]
    public void Editor_ReplaceTimeline()
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
