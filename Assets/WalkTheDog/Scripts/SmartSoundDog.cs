namespace ToyBoxHHH
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Does a basic setup of AudioSource and provides some simple features that I often use when doing sound.
    /// Ideally extended in every project with its specific sound needs, since they are always a little bit varied.
    /// 
    /// made by @horatiu665
    /// </summary>
    public class SmartSoundDog : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audio;
        public new AudioSource audio
        {
            get
            {
                if (_audio == null)
                {
                    _audio = GetComponent<AudioSource>();
                }
                return _audio;
            }
        }

        public List<AudioClip> clips = new List<AudioClip>();

        public bool playRandomClip = true;

        public bool doNotInterrupt = false;

        private AudioClip currentClip;

        public Vector2 randomPitchRange = new Vector2(1, 1f);


        void Reset()
        {
            _audio = GetComponent<AudioSource>();
            if (_audio == null)
            {
                _audio = gameObject.AddComponent<AudioSource>();
            }
            _audio.playOnAwake = false;
            _audio.spatialBlend = 1f;
            _audio.dopplerLevel = 0f;

        }

        private void OnValidate()
        {
            if (audio != null)
            {
            }
        }

        private void Awake()
        {
            if (audio != null)
            {
            }
        }


        [DebugButton]
        public void Play()
        {
            if (doNotInterrupt)
            {
                if (_audio.isPlaying)
                {
                    return;
                }
            }

            if (playRandomClip)
            {
                if (clips.Count > 0)
                {
                    _audio.clip = clips[Random.Range(0, clips.Count)];
                }
            }

            if (randomPitchRange.x != 1f || randomPitchRange.y != 1f)
                _audio.pitch = Random.Range(randomPitchRange.x, randomPitchRange.y);

            _audio.Play();
        }

        [DebugButton]
        public void PlayOneShot()
        {

            if (playRandomClip)
            {
                if (clips.Count > 0)
                {
                    currentClip = clips[Random.Range(0, clips.Count)];
                }
            }
            else
            {
                currentClip = clips[0];
            }

            _audio.PlayOneShot(currentClip);
        }

        public void Stop()
        {
            if (_audio.isPlaying)
            {
                _audio.Stop();
            }
        }
    }
}