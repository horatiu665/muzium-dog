using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class DogBarkAnim : MonoBehaviour
{
    public bool activated = false;

    public AudioSource dogSound;

    public float envelope { get; private set; }

    [Space]
    public DogBarkSettings settings;

    public Transform dogToMove;

    private Vector3 prevOffset;
    private Vector3 smoothOffset;

    public bool playRandomSounds = true;

    public float maxDistanceFromPlayer = 10f;
    public float maxDistanceFromDawg = 10f;

    private SmartSoundDog _smartSoundDog;
    public SmartSoundDog smartSoundDog
    {
        get
        {
            if (_smartSoundDog == null)
            {
                _smartSoundDog = GetComponent<SmartSoundDog>();
            }
            return _smartSoundDog;
        }
    }

    void Update()
    {
        if (DogControlPanel.instance.dog.dogBrain.player == null)
            return;

        if (!activated)
        {
            ElegantStop();
            return;
        }

        var distToPlayer = Vector3.Distance(dogToMove.position, DogControlPanel.instance.dog.dogBrain.player.position);
        var distToDog = Vector3.Distance(dogToMove.position, DogControlPanel.instance.dog.transform.position);
        if (distToPlayer > maxDistanceFromPlayer && distToDog > maxDistanceFromDawg)
        {
            ElegantStop();
            return;
        }

        // animate
        var newOffset =
            Vector3.up * settings.upCurve.Evaluate(envelope) * settings.upAmount
            + Random.insideUnitSphere * settings.shakeAmount * settings.shakeCurve.Evaluate(envelope);

        smoothOffset = Vector3.Lerp(smoothOffset, newOffset, settings.smoothness);
        dogToMove.position += smoothOffset - prevOffset;
        prevOffset = smoothOffset;


        if (playRandomSounds)
        {
            if (!smartSoundDog.audio.isPlaying)
                smartSoundDog.Play();
        }
    }

    private void ElegantStop()
    {
        smoothOffset = Vector3.zero;
        dogToMove.position += smoothOffset - prevOffset;
        prevOffset = Vector3.zero;

        smartSoundDog.audio.Pause();
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
