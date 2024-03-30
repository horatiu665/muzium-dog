using System;
using UnityEngine;

[Serializable]
public class AudioFloat
{
    public float value;
    private float valueMax;
    private int dir;
    private float dirTimer;
    public float dropDelay = 0.1f;
    [Range(0, 1f)]
    public float dropFactor = 0.99f;
    public float smooth = 0.2f;

    public void Update(float newValue)
    {
        // if the new value is higher than the current value, start growing the envelope max
        if (valueMax < newValue)
        {
            dir = 1;
            valueMax = Mathf.Max(newValue, valueMax);
            dirTimer = Time.time + dropDelay;
        }
        // if the new value is lower than the current value, shrink the envelope max slowly
        else
        {
            dir = -1;
            // enough time has passed, start shrinking the envelope max
            if (Time.time > dirTimer)
            {
                valueMax = valueMax * dropFactor;
            }
        }

        value = Mathf.Lerp(value, valueMax, smooth);

    }

}