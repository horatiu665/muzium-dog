using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] public AudioSource _musicSource, _effectsSource, _playerSource;

    public float menuMusicVolume = 1;

    void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

    }

    public void PlaySound(AudioClip clip){
        _effectsSource.PlayOneShot(clip);
    }

    public void PlayPlayerSound(AudioClip clip){
        _playerSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, int loop){
        _musicSource.PlayOneShot(clip);
        if (loop == 1) _musicSource.loop = true;
        if (loop == 0) _musicSource.loop = false;
    }

    public void StopAllMusic()
    {
        _musicSource.Stop();
    }

    public void StopSound()
    {
        _effectsSource.Stop();
    }

    public void ChangeMasterVolume(float value){
        AudioListener.volume = value;
    }
    public void ChangeMusicVolume(float value){
        _musicSource.volume = value;
    }
    public void ChangeEffectVolume(float value){
        _effectsSource.volume = value;
    }
}
