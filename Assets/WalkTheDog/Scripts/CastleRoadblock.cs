using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleRoadblock : MonoBehaviour
{
    public DogConcert dogConcert;
    public Transform player => DogControlPanel.instance.dog.dogBrain.player;

    public Transform resetPlayer;

    public RectTransform roadblockUI;

    public KeyCode hideOnKeypress = KeyCode.P;

    public GameObject[] activeWhenBlocked;
    public GameObject showThatItIsBlocked;

    public ConcertCandleSystem concertCandleSystem;

    public bool teleportPlayer = false;

    public bool isBlocked
    {
        get
        {
            // we can use the hidden state of the concert to allow access to the castle.
            // and we can toggle the concert state based on the candle system.
            if (dogConcert.dogConcertHideShow.concertState == DogConcertHideShow.ConcertState.Hidden)
                return false;

            return true;
            //return !concertCandleSystem.AreAllArtworksCompleted();
        }
    }

    private void OnEnable()
    {
        dogConcert.OnPlayerEnterConcertRadius += OnPlayerEnterConcertRadius;
        SetUI(false);
    }

    private void OnDisable()
    {
        dogConcert.OnPlayerEnterConcertRadius -= OnPlayerEnterConcertRadius;
    }

    private void OnPlayerEnterConcertRadius()
    {
        RefreshRoadblockState();
    }

    public void RefreshRoadblockState()
    {
        foreach (var go in activeWhenBlocked)
        {
            go.SetActive(isBlocked);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (teleportPlayer)
        {
            if (other.transform == player)
            {
                player.position = resetPlayer.position;
                player.rotation = resetPlayer.rotation;
            }
        }

        // show the UI about the roadblock
        SetUI(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(hideOnKeypress))
        {
            SetUI(false);
        }

    }

    void SetUI(bool visible)
    {
        roadblockUI.gameObject.SetActive(visible);
    }

}
