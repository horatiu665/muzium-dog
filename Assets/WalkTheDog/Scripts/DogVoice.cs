using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class DogVoice : MonoBehaviour
{

    [SerializeField]
    private DogRefs _dogRefs;
    public DogRefs dogRefs
    {
        get
        {
            if (_dogRefs == null)
            {
                _dogRefs = GetComponentInParent<DogRefs>();
            }
            return _dogRefs;
        }
    }

    public DogMouthBrain mouthBrain => dogRefs.dogBrain.dogMouthBrain;

    public SmartSoundDog barkAll;
    public AudioClip[] barkIntensities;

    public AudioClip[] barkHappy;
    public AudioClip[] barkAngry;
    public AudioClip[] barkNormal;

    public SmartSoundDog whimper;
    public SmartSoundDog growl;
    private float growlTargetVol;

    public SmartSoundDog pant;
    private float pantTargetVol;

    public SmartSoundDog sniff;
    private float sniffTargetVol;

    public SmartSoundDog howl;
    public SmartSoundDog wag;

    public SmartSoundDog footsteps;

    private float duckAllSoundsTime;

    [DebugButton]
    public void BarkAny()
    {
        barkAll.playRandomClip = true;
        barkAll.Play();

        duckAllSoundsTime = Time.time;
        mouthBrain.Bark();
    }

    public void BarkHappy()
    {
        barkAll.audio.clip = barkHappy[Random.Range(0, barkHappy.Length)];
        barkAll.playRandomClip = false;
        barkAll.Play();

        duckAllSoundsTime = Time.time;
        mouthBrain.Bark();

    }

    public void BarkAngry()
    {
        barkAll.audio.clip = barkAngry[Random.Range(0, barkAngry.Length)];
        barkAll.playRandomClip = false;
        barkAll.Play();

        duckAllSoundsTime = Time.time;
        mouthBrain.Bark();

    }

    public void BarkNormal()
    {
        barkAll.audio.clip = barkNormal[Random.Range(0, barkNormal.Length)];
        barkAll.playRandomClip = false;
        barkAll.Play();

        duckAllSoundsTime = Time.time;
        mouthBrain.Bark();

    }

    public void BarkIntensity(float intensity01)
    {
        int index = Mathf.FloorToInt(intensity01 * (barkIntensities.Length - 1));
        barkAll.audio.clip = barkIntensities[index];
        barkAll.playRandomClip = false;
        barkAll.Play();

        duckAllSoundsTime = Time.time;
        mouthBrain.Bark();

    }


    public void Sniff(float vol)
    {
        if (!sniff.audio.isPlaying)
        {
            sniff.Play();
        }
        sniffTargetVol = vol;

        //mouthBrain.MildMouthMovement(vol > 0);

    }

    public void Pant(float vol)
    {
        if (!pant.audio.isPlaying)
        {
            pant.Play();
        }
        pantTargetVol = vol;

        mouthBrain.Pant(vol > 0);

    }

    public void Growl(float vol)
    {
        if (!growl.audio.isPlaying)
        {
            growl.Play();
        }
        growlTargetVol = vol;

        mouthBrain.MildMouthMovement(vol > 0);

    }

    public void Whimper()
    {
        whimper.Play();

        duckAllSoundsTime = Time.time;

        mouthBrain.MildMouthMovement(true);
        StartCoroutine(pTween.Wait(whimper.audio.clip.length, () => mouthBrain.MildMouthMovement(false)));

    }

    public void Footstep()
    {
        footsteps.Play();
    }


    void Update()
    {
        // when duckAllSoundsTime was just set, duckVolumeOffset shoudl be 1. then it should go down to 0.
        var duckVolumeOffset = Mathf.Clamp01(1 - (Time.time - duckAllSoundsTime));

        sniff.audio.volume = Mathf.Lerp(sniff.audio.volume, sniffTargetVol - duckVolumeOffset, 0.4f);
        pant.audio.volume = Mathf.Lerp(pant.audio.volume, pantTargetVol - duckVolumeOffset, 0.4f);

    }

}
