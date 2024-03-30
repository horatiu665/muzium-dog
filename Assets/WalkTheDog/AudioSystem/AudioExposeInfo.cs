using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioExposeInfo : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioSource audioSource
    {
        get
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
            return _audioSource;
        }
    }

    public float envelope { get; private set; }

    public AudioFloat envSmooth = new AudioFloat();
    public AudioFloat envLo = new AudioFloat();
    public AudioFloat envHi = new AudioFloat();

    public enum EnvVariant {
        Current,
        Classic,
        Lo,
        Hi,
    }
    public float GetEnv(EnvVariant variant)
    {
        switch (variant)
        {
            case EnvVariant.Current:
                return envelope;
            case EnvVariant.Classic:
                return envSmooth.value;
            case EnvVariant.Lo:
                return envLo.value;
            case EnvVariant.Hi:
                return envHi.value;
            default:
                return 0;
        }
    }

    // frequency threshold for what it means Lo or Hi
    [Range(0, 1f)]
    public float fftLoHiThreshold01 = 0.5f;
    public int fftCount = 64;
    public float[] fftSpectrum { get; private set; }


    private void OnEnable()
    {
        fftSpectrum = new float[fftCount];
    }

    void Update()
    {
        envSmooth.Update(envelope);

        Update_SpectrumData();
    }


    private void OnAudioFilterRead(float[] data, int channels)
    {
        // calc envelope
        float sum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            sum += Mathf.Abs(data[i]);
        }
        envelope = sum / data.Length;

    }

    void Update_SpectrumData()
    {
        this.audioSource.GetSpectrumData(fftSpectrum, 0, FFTWindow.Rectangular);

        float fftLoHiThreshold = this.fftLoHiThreshold01 * this.fftCount;
        // find the max low/high freq from spectrum
        float maxLo = 0;
        float maxHi = 0;
        for (int i = 0; i < fftLoHiThreshold; i++)
        {
            maxLo = Mathf.Max(maxLo, fftSpectrum[i]);
        }
        for (int i = (int)fftLoHiThreshold; i < fftCount; i++)
        {
            maxHi = Mathf.Max(maxHi, fftSpectrum[i]);
        }

        envLo.Update(maxLo);
        envHi.Update(maxHi);
        
    }
}