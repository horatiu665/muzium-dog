using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class ConcertCandleSystem : MonoBehaviour
{
    public DogConcert dogConcert;

    public List<bool> artworkCompleted = new();

    public List<Candle> candles = new List<Candle>();

    public bool AreAllArtworksCompleted()
    {
        for (int i = 0; i < artworkCompleted.Count; i++)
        {
            if (!artworkCompleted[i])
            {
                return false;
            }
        }

        return true;
    }

    public bool IsArtworkCompleted(int artworkId)
    {
        if (artworkId < artworkCompleted.Count)
        {
            return artworkCompleted[artworkId];
        }

        return false;
    }

    private void OnEnable()
    {
        dogConcert.OnPlayerEnterConcertRadius += OnPlayerEnterConcertRadius;
        SetAllCandlesToArtworksStatus();
    }

    private void OnDisable()
    {
        dogConcert.OnPlayerEnterConcertRadius -= OnPlayerEnterConcertRadius;

    }

    private void OnPlayerEnterConcertRadius()
    {
        SetAllCandlesToArtworksStatus();
    }

    /// <summary>
    /// Sets candles from list to match artwork ID 1..N where N is the number of candles.
    /// </summary>
    [DebugButton]
    public void Editor_SetCandles1N()
    {
        var foundCandles = GetComponentsInChildren<Candle>();
        for (int i = 0; i < foundCandles.Length; i++)
        {
            if (!candles.Contains(foundCandles[i]))
            {
                candles.Add(foundCandles[i]);
            }
        }

        candles.Shuffle();

        for (int i = 0; i < candles.Count; i++)
        {
            candles[i].artworkId = i;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(candles[i]);
#endif
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    [DebugButton]
    public void SetAllCandlesToArtworksStatus()
    {
        foreach (var candle in candles)
        {
            bool isCompleted = IsArtworkCompleted(candle.artworkId);
            // candles are extinguished when artworks have been visited.
            bool shouldBeOn = !isCompleted;

            candle.Set(shouldBeOn);
        }


        // dirty candles
        for (int i = 0; i < candles.Count; i++)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(candles[i]);
#endif
        }

    }




}
