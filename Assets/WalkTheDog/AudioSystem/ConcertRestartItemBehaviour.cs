using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcertRestartItemBehaviour : ItemBehaviour
{
    public DogConcert dogConcert;

    public float concertStartDelaySeconds = 60;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnFocus()
    {
        base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();
    }

    public override void OnInteract()
    {
        base.OnInteract();

        // if concert is not playing, schedule concert to start 
        if (dogConcert.dogConcertHideShow.concertState == DogConcertHideShow.ConcertState.Hidden)
        {
            PoemSystem.instance.ShowCustomText("Take your seats! \nThe concert is scheduled to start\n in about 1 minute!", true);

            StopAllCoroutines();

            PoemSystem.instance.OnPoemHidden += OnPoemHidden;

        }
        else
        {
            PoemSystem.instance.ShowCustomText("The concert seems to be in progress!", true);

        }

    }

    private void OnPoemHidden()
    {
        PoemSystem.instance.OnPoemHidden -= OnPoemHidden;

        dogConcert.dogConcertHideShow.SetConcertState(DogConcertHideShow.ConcertState.Starting);

        StartCoroutine(pTween.Wait(concertStartDelaySeconds, () =>
        {
            dogConcert.dogConcertHideShow.SetConcertState(DogConcertHideShow.ConcertState.Playing);

        }));
    }

}
