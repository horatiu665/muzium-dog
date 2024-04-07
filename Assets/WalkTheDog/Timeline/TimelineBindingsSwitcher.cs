using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

public class TimelineBindingsSwitcher : MonoBehaviour
{
    public PlayableDirector playableDirector;

    private Dictionary<Object, Object> trackSourcesToBindings = new Dictionary<Object, Object>();

    private List<PlayableBinding> savedOutputs = new List<PlayableBinding>();

    [Header("Debug")]
    public TimelineAsset editor_whichToSwitchTo;


    [DebugButton]
    public void Editor_SwitchToTimeline()
    {
        SwitchEmWithRefs(editor_whichToSwitchTo);
    }

    public void SwitchEmWithoutRefs(TimelineAsset newTimeline)
    {
        playableDirector.playableAsset = newTimeline;

    }
    // This function assumes a current timeline is assigned, with the correct references on the tracks.
    // The newTimeline must have the same outputs as the current timeline, for it to be able to switch
    public void SwitchEmWithRefs(TimelineAsset newTimeline)
    {
        trackSourcesToBindings.Clear();

        // the old timeline is: 
        var curTimeline = playableDirector.playableAsset;

        // outputs from the old timeline
        savedOutputs = curTimeline.outputs.ToList();

        // now we should assign the new timeline
        playableDirector.playableAsset = newTimeline;

        var newTimelineOutputs = playableDirector.playableAsset.outputs.ToList();
        // set outputs of the new timeline
        for (int i = 0; i < newTimelineOutputs.Count; i++)
        {
            if (!newTimelineOutputs[i].sourceObject) continue;
            if (savedOutputs.Count < i)
                continue;
            
            playableDirector.SetGenericBinding(
                newTimelineOutputs[i].sourceObject,
                playableDirector.GetGenericBinding(savedOutputs[i].sourceObject));
        }

        // IT WORKS!!! <3
        // actually no. </3
        // only when the timeline has been freshly copied and all things are in the same order exactly. after changing anything it is likely to break.

        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(playableDirector);
#endif
    }
}