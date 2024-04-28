using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudienceGroove : MonoBehaviour
{
    public static PianoPlayerCharacterController pianoPlayer;

    public enum Methods
    {
        WagX,
        WagY,
        WagZ,
        Jump
    }
    public Methods method;

    public float wagSinMultiplier = 1f;
    public float wagAmount = 10f;
    public float wagPhase = 0f;

    private Quaternion initRotation;

    public bool randomize = true;

    [FormerlySerializedAs("audienceAudioSource")]
    public AudioSource clapAudioSource;
    public AnimationCurve clapMovement = AnimationCurve.Linear(0, 0, 1, 1);


    private void OnEnable()
    {
        if (pianoPlayer == null)
        {
            pianoPlayer = FindObjectOfType<PianoPlayerCharacterController>();
        }
        initRotation = transform.localRotation;

        if (randomize)
        {
            wagPhase = Random.Range(0f, 360f);
            wagSinMultiplier = Mathf.RoundToInt(Random.Range(1, 4));
            method = Random.value < 0.2f ? Methods.Jump : Random.value < 0.5f ? Methods.WagZ : Random.value < 0.5f ? Methods.WagY : Methods.WagX;
            if (method == Methods.Jump)
            {
                wagAmount = Random.Range(0.1f, 0.2f);
            }
            else
            {
                wagAmount = Random.Range(3f, 7f);
            }
        }
    }
    void Update()
    {
        var wagData = pianoPlayer.GetTorsoWag();
        switch (method)
        {
            case Methods.WagX:
                transform.localRotation = initRotation * Quaternion.Euler(Mathf.Sin(wagSinMultiplier * wagData.wagT + wagPhase) * wagAmount, 0, 0);
                break;
            case Methods.WagY:
                transform.localRotation = initRotation * Quaternion.Euler(0, Mathf.Sin(wagSinMultiplier * wagData.wagT + wagPhase) * wagAmount, 0);
                break;
            case Methods.WagZ:
                transform.localRotation = initRotation * Quaternion.Euler(0, 0, Mathf.Sin(wagSinMultiplier * wagData.wagT + wagPhase) * wagAmount);
                break;
            case Methods.Jump:
                transform.localPosition = new Vector3(0, wagData.wagDelta * wagAmount, 0);
                break;
        }

        if (clapAudioSource.isPlaying)
        {
            transform.localPosition = Vector3.up * clapMovement.Evaluate(clapAudioSource.time);
        } else
        {
            transform.localPosition = Vector3.zero;
        }
    }
}
