using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DogConcert : MonoBehaviour
{

    public PlayableDirector concertTimeline;
    public TimelineAsset replaceWith;

    public bool useCheat = false;
    public KeyCode cheatKey = KeyCode.K;

    private void Update() {
        if (useCheat && Input.GetKeyDown(cheatKey))
        {
            RestartConcert();
        }
    }

    [DebugButton]
    public void RestartConcert()
    {
        // if (!concertTimeline.playableGraph.IsPlaying() || !concertTimeline.playableGraph.IsValid())
        // {
        //     concertTimeline.Play();
        // }

        concertTimeline.time = 0;
        concertTimeline.Play();
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);

    }

    [DebugButton]
    public void EndConcert()
    {
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(0);


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
}
