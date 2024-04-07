using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class CandleSpawner : MonoBehaviour
{
    public List<Transform> spawnedCandles = new List<Transform>();
    public Vector2 randomScaleRange = new Vector2(0.5f, 1.5f);

    [DebugButton]
    public void RandomizeScale()
    {
        foreach (var candle in spawnedCandles)
        {
            candle.localScale = Vector3.one * Random.Range(randomScaleRange.x, randomScaleRange.y);
        }
    }
}
