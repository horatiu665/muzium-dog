using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[System.Serializable]
public class Track : MonoBehaviour
{
    [HideInInspector]
    public AudioSource track = new AudioSource();

    public AudioClip myAudioClip;

    float progress = 1f;
    float fromValue = 0f;
    float toValue = 1f;
    float fadeDuration = 1f;
    [HideInInspector]
    public int isFading = 0;

    AudioLoom myAudioLoom;

    [HideInInspector]
    public float maxVolume = 1f;

    public bool canPlayInRevert;

   [HideInInspector]
    public List<int> playableDepths = new List<int>();

    float prevVolume = 0f;
    float myVolume = 0f;


    void Awake()
    {
        track = gameObject.AddComponent<AudioSource>() as AudioSource;
        track.clip = myAudioClip;
        track.loop = true;
        track.volume = 0f;
        myAudioLoom = transform.parent.GetComponent<AudioLoom>();
        playableDepths = new List<int>();
        SetAudioSourceParameters(transform.parent.gameObject.GetComponent<AudioSource>());
    }

    void SetAudioSourceParameters(AudioSource model)
    {
        if (GetComponent<AudioSource>() == null)
            return;

        track.outputAudioMixerGroup = model.outputAudioMixerGroup;
        track.mute = model.mute;
        track.bypassEffects = model.bypassEffects;
        track.bypassListenerEffects = model.bypassListenerEffects;
        track.bypassReverbZones = model.bypassReverbZones;
        track.priority = model.priority;
        track.panStereo = model.panStereo;
        track.spatialBlend = model.spatialBlend;
        track.reverbZoneMix = model.reverbZoneMix;
        track.dopplerLevel = model.dopplerLevel;
        track.spread = model.spread;
        track.rolloffMode = model.rolloffMode;
        track.minDistance = model.minDistance;
        track.maxDistance = model.maxDistance;
        track.SetCustomCurve(AudioSourceCurveType.CustomRolloff, model.GetCustomCurve(AudioSourceCurveType.CustomRolloff)); 
    }
    
    void Start()
    {

    }

    public void AddPlayableState(int val)
    {
        if (playableDepths.Contains(val) == false)
            playableDepths.Add(val);
    }


    void Update()
    {
        if (progress < 1f && isFading!=0)
        {
            progress += Time.deltaTime / fadeDuration;
            if (progress >= 1f)
            {
                progress = 1f;
                isFading = 0;
            }
               
            myVolume = Mathf.Lerp(fromValue, toValue, progress);

            if (myVolume <= 0f)
            {
                track.Stop();
                maxVolume = 1f;
                track.pitch = 1f;
            }
                
        }


        UpdateVolume();
    }

    void UpdateVolume()
    {
        if(Mathf.Abs(prevVolume- myVolume* myAudioLoom.globalVolume)>0.001f)
        {
            prevVolume = myVolume * myAudioLoom.globalVolume;
            track.volume = myVolume * myAudioLoom.globalVolume;
        }

        

    }

    public bool CheckDepthCondition(int check)
    {
        return playableDepths.Count == 0 || playableDepths.Contains(check);
    }

   

    public bool isPlayingOrFadingIn()
    {
        if (track.isPlaying)
            return true;

        if (isFading > 0)
            return true;

        return false;
    }

    public void FadeIn(float duration =-1f, bool forceFade = false)
    {
        if(!track.isPlaying)
        {
                track.time = Random.Range(0f, track.clip.length);

                if (canPlayInRevert && myAudioLoom.CurrentRangeProperties().revertProbability > Random.value)
                {
                    track.pitch = -1f;
                    maxVolume = 0.5f;
                }
                else
                {
                    track.pitch = 1f;
                    maxVolume = 1f;
                }
        }
 
        FromThisValueToThatValue(track.volume, maxVolume, duration);
    }
    
    public void FadeOut(float duration = -1f,bool forceFade = false)
    {
        FromThisValueToThatValue(track.volume, 0f, duration, forceFade);
    }

    public void FromThisValueToThatValue(float valueA, float valueB, float duration = -1f,bool forceFade = false)
    {
        if (duration <= 0f)
            duration = 1f;

        if (Mathf.Approximately(valueA, valueB))
            return;

        if ((isFading == ((valueB > valueA) ? 1 : -1)) && !forceFade)
            return;

        isFading = (valueB > valueA) ? 1 : -1;
        fadeDuration = duration;
        fromValue = valueA;
        toValue = valueB;
        progress = 0f;
        track.volume = valueA;

        if (!track.isPlaying)
            track.Play();
    }

    public void InstantStop()
    {
        isFading = 0;

        fromValue = 1f;
        toValue = 0f;
        progress = 1f;
        track.volume = 0f;
        if (track.isPlaying)
            track.Stop();
    }

    public void InstantPlay()
    {
        isFading = 0;

        fromValue = 0f;
        toValue = 1f;
        progress = 1f;
        track.volume = 1f;
        track.Play();

            
    }
}
