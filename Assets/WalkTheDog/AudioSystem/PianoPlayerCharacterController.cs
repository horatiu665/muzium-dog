// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoPlayerCharacterController : MonoBehaviour
{
    public AudioExposeInfo audioPA, audioMouth, audioPiano;

    public Transform head, jaw, armL, armR, body, legs, tail;
    public Transform headParentForward;

    public Animator pianistAnim;

    [Space]
    public AudioExposeInfo.EnvVariant mouthEnvVariant;
    public float mouthAmplitude = 1f;
    public AudioExposeInfo.EnvVariant mouthEnvHiVariant;
    public float mouthHiAmplitude = 1f;
    public AudioExposeInfo.EnvVariant pianoEnvVariant;
    public float pianoAmplitude = 1f;
    public AudioExposeInfo.EnvVariant pianoEnvLoVariant;
    public float pianoLoAmplitude = 1f;
    public AudioExposeInfo.EnvVariant pianoEnvHiVariant;
    public float pianoHiAmplitude = 1f;

    [Space]
    public float tailWagSpeed = 0f;
    public float tailWagAmplitude = 0f;
    private float tailWagT;

    public AudioExposeInfo torsoAudioExposer;
    public AudioExposeInfo.EnvVariant torsoEnvAdvanceVariant;

    private float torsoWagT;
    private float torsoWagDelta;
    public float torsoWagAdvance = 1f;
    private float torsoWagFinal;
    public struct WagData {
        public float wagDelta;
        public float wagT;
        public float wagFinal;
    }

    public WagData GetTorsoWag()
    {
        return new WagData
        {
            wagDelta = torsoWagDelta,
            wagT = torsoWagT,
            wagFinal = torsoWagFinal
        };
    }

    [Space]
    public List<Transform> headLookTargets = new List<Transform>();
    public bool headLookAutoAdd = true;
    private Transform headLookCurTarget;
    private Vector3 headLookTargetPosition;
    private Vector3 headLookTargetRandomOffset;
    public Vector2 headLookTargetChangeTimeRange = new Vector2(1, 3);
    private float headLookNextChangeTime;
    private float headLookMoveHeadTime = 0;
    public float headLookMaxAngleFromForward = 45;

    void OnEnable()
    {
        if (headLookAutoAdd)
        {
            headLookTargets.Add(DogControlPanel.instance.dog.head);
            headLookTargets.Add(DogControlPanel.instance.dog.dogBrain.mainCamera.transform);

        }

        headLookCurTarget = headLookTargets[0];
        headLookTargetPosition = headLookCurTarget.position;

    }

    void Update()
    {
        pianistAnim.SetFloat("MouthEnv", audioMouth.GetEnv(mouthEnvVariant) * mouthAmplitude);
        pianistAnim.SetFloat("MouthEnvHi", audioMouth.GetEnv(mouthEnvHiVariant) * mouthHiAmplitude);
        pianistAnim.SetFloat("PianoEnv", audioPiano.GetEnv(pianoEnvVariant) * pianoAmplitude);
        pianistAnim.SetFloat("PianoEnvLo", audioPiano.GetEnv(pianoEnvLoVariant) * pianoLoAmplitude);
        pianistAnim.SetFloat("PianoEnvHi", audioPiano.GetEnv(pianoEnvHiVariant) * pianoHiAmplitude);

        tailWagT += Time.deltaTime * tailWagSpeed;
        tail.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(tailWagT) * tailWagAmplitude);

        var torsoWag = torsoAudioExposer.GetEnv(torsoEnvAdvanceVariant) * torsoWagAdvance;
        torsoWagDelta = torsoWag;
        torsoWagT += Time.deltaTime * torsoWag;
        torsoWagFinal = Mathf.Sin(torsoWagT);
        pianistAnim.SetFloat("TorsoWag", torsoWagFinal);

        Update_HeadTarget();
        head.LookAt(headLookTargetPosition);

    }

    private void Update_HeadTarget(int monteCarlo = 5)
    {
        if (monteCarlo <= 0)
        {
            return;
        }
        // should we change?
        bool shouldChangeTarget = false;
        if (Time.time > headLookNextChangeTime)
        {
            shouldChangeTarget = true;
        }

        // angle
        var angle = Vector3.Angle(headParentForward.forward, headLookCurTarget.position - headParentForward.position);
        if (angle > headLookMaxAngleFromForward)
        {
            shouldChangeTarget = true;
        }

        if (shouldChangeTarget || headLookCurTarget == null)
        {
            headLookNextChangeTime = Time.time + UnityEngine.Random.Range(headLookTargetChangeTimeRange.x, headLookTargetChangeTimeRange.y);
            headLookCurTarget = headLookTargets[UnityEngine.Random.Range(0, headLookTargets.Count)];
            headLookTargetRandomOffset = Random.onUnitSphere * 0.2f;
            headLookMoveHeadTime = Time.time + 0.2f;
        }

        if (Time.time > headLookMoveHeadTime)
        {
            headLookTargetPosition = Vector3.Lerp(headLookTargetPosition, headLookCurTarget.position + headLookTargetRandomOffset, 0.1f);
        }
    }
}
