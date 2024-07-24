using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    public Candle candle;

    public Light light;

    private float initIntensity;
    private float _t;
    public float speed = 10f;
    public float flickerAmount = 0.1f;
    private float randomPhase = 0f;

    private void Awake()
    {
        candle = GetComponentInParent<Candle>();
        initIntensity = light.intensity;
        randomPhase = Random.Range(0, 100f);
        _t += randomPhase;
    }

    void Update()
    {
        if (candle.isOn)
        {
            _t += Time.deltaTime * speed;
            light.enabled = true;
            light.intensity = initIntensity + (Mathf.Sin(_t) + Mathf.Sin(_t * 0.467f) * 0.5f) * flickerAmount;
        }
        else
        {
            light.enabled = false;
        }
    }
}
