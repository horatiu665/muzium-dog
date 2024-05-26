using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDistanceParam : MonoBehaviour
{
    public string parameterName;
    AudioListener _audioListener;
    public AudioListener audioListener
    {
        get
        {
            if (_audioListener == null || !_audioListener.isActiveAndEnabled)
            {
                _audioListener = FindObjectOfType<AudioListener>();
            }
            return _audioListener;
        }
    }
    public Transform target => audioListener.transform;
    public Transform source => this.transform;
    public float maxDistance = 10;
    public float minDistance = 1;

    private AudioSource audioSource;

    public AnimationCurve dist01ToParam = AnimationCurve.Linear(0, 0, 1, 1);

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(target.position, source.position);
        if (float.IsNaN(distance))
            return;

        float distance01 = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));

        audioSource.outputAudioMixerGroup.audioMixer.SetFloat(parameterName, dist01ToParam.Evaluate(distance01));
    }
}
