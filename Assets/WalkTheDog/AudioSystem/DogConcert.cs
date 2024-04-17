using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DogConcert : MonoBehaviour
{
    public Transform player => DogCastleReferences.instance.dogBrain.player;

    public PlayableDirector concertTimeline;
    public TimelineAsset replaceWith;

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

    private void Update()
    {
        // cheat
        if (useCheat && Input.GetKeyDown(cheatKey))
        {
            RestartConcert();
        }

        // check if player is close enough to hear the concert
        var playerPos = player.position;

        // TODO: consider up/down distance 
        var dist = Vector3.Distance(playerPos, concertCenter.position);

        if (!playerIsInConcertRange && dist < concertHearingThreshold)
        {
            playerIsInConcertRange = true;
            playerEnterTime = Time.time;
        }
        if (playerIsInConcertRange && dist > concertHearingThreshold + 1)
        {
            playerIsInConcertRange = false;
            playerLeaveTime = Time.time;
        }

        // pause and resume the concert based on player's presence
        if (playerIsInConcertRange)
        {
            ContinueConcertSmart();
        }
        else if (!playerIsInConcertRange)
        {
            if (IsConcertPlaying())
            {
                var timeSincePlayerLeft = Time.time - playerLeaveTime;
                if (timeSincePlayerLeft > timeUntilStopMusicWhenLeaving)
                {
                    PauseConcert();
                }
            }
        }

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
        if (!IsConcertPlaying())
        {
            ResumeConcert();
        }
    }

    public bool IsConcertPlaying()
    {
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
