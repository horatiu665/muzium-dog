using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class ConcertCandleSystem : MonoBehaviour
{
    public List<Candle> candles = new List<Candle>();

    public bool IsArtworkCompleted(int artworkId)
    {
        // todo: implement with Zium code.
        return Random.value > 0.5f;

    }

    private void OnEnable()
    {
        SetAllCandlesToArtworksStatus();
    }

    [DebugButton]
    public void Util_SetCandles1_N()
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
