using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Playables;

public class DogConcert : MonoBehaviour
{

    public PlayableDirector concertTimeline;

    [DebugButton]
    public void RestartConcert()
    {
        concertTimeline.time = 0;
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
        concertTimeline.Play();

    }

    [DebugButton]
    public void EndConcert()
    {
        concertTimeline.playableGraph.GetRootPlayable(0).SetSpeed(0);


    }


}
