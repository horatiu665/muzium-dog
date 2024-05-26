using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcertRestartItemBehaviour : ItemBehaviour
{
    public Collider thisCollider;

    public DogConcert dogConcert;

    public float concertStartDelaySeconds = 5;

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

            // risky move. but it's a hack so we don't trigger the poem system multiple times upon clicking.
            // TODO: proper fix from ZIUM system, so the poem system can inhibit the interactable (and movement) controls.
            thisCollider.enabled = false;

        }
        else
        {
            PoemSystem.instance.ShowCustomText("The concert seems to be in progress!", true);
            thisCollider.enabled = false;
            PoemSystem.instance.OnPoemHidden += ReenableSelf;

        }

    }

    private void ReenableSelf()
    {
        PoemSystem.instance.OnPoemHidden -= ReenableSelf;
        thisCollider.enabled = true;
    }

    private void OnPoemHidden()
    {
        PoemSystem.instance.OnPoemHidden -= OnPoemHidden;

        thisCollider.enabled = true;

        dogConcert.dogConcertHideShow.SetConcertState(DogConcertHideShow.ConcertState.Starting);

        StartCoroutine(pTween.Wait(concertStartDelaySeconds, () =>
        {
            dogConcert.dogConcertHideShow.SetConcertState(DogConcertHideShow.ConcertState.Playing);

            dogConcert.RestartMainConcertFromTicketOffice();

        }));
    }

}
