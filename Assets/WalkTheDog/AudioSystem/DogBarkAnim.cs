using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class DogBarkAnim : MonoBehaviour
{
    public AudioSource dogSound;

    public float envelope { get; private set; }

    [Space]
    public DogBarkSettings settings;


    public Transform dogToMove;

    private Vector3 prevOffset;
    private Vector3 smoothOffset;

    public bool playRandomSounds = true;

    void Update()
    {
        // animate
        var newOffset =
            Vector3.up * settings.upCurve.Evaluate(envelope) * settings.upAmount
            + Random.insideUnitSphere * settings.shakeAmount * settings.shakeCurve.Evaluate(envelope);

        smoothOffset = Vector3.Lerp(smoothOffset, newOffset, settings.smoothness);
        dogToMove.position += smoothOffset - prevOffset;
        prevOffset = smoothOffset;


        if (playRandomSounds)
        {
            var ssd = GetComponent<SmartSoundDog>();

            if (!ssd.audio.isPlaying)
                ssd.Play();
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        // compute envelope
        envelope = 0;
        for (int i = 0; i < data.Length; i++)
        {
            envelope = Mathf.Max(envelope, Mathf.Abs(data[i]));
        }
    }


}
