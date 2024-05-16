using System;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class DogConcertAudience : MonoBehaviour
{

    public Transform targetAtConcert;
    public Transform targetHidden;

    public bool isAtConcert = false;

    public float walkSpeed = 1f;

    public float steppingSin = 2f;
    public float steppingRotSin = 2f;
    public float stepHeight = 0.1f;

    public float thresholdForStartMoving = 0.5f;
    public float thresholdForStopMoving = 0.1f;
    private bool isMoving;

[Space]
    public bool toggleRandomDogOnEnable = true;

    public List<AudienceGroove> whichDog = new();

    public void ToggleRandomDog()
    {
        int randomIndex = UnityEngine.Random.Range(0, whichDog.Count);
        for (int i = 0; i < whichDog.Count; i++)
        {
            whichDog[i].gameObject.SetActive(i == randomIndex);
        }
    }

    [Header("Debug")]
    public bool moveHiddenPositions = false;


    [DebugButton]
    public void SetupTargets()
    {
        if (targetAtConcert == null)
        {
            targetAtConcert = new GameObject("TargetAtConcert").transform;
            targetAtConcert.SetParent(transform);
        }
        targetAtConcert.position = transform.position;
        targetAtConcert.rotation = transform.rotation;
        if (targetHidden == null)
        {
            targetHidden = new GameObject("TargetHidden").transform;
            targetHidden.SetParent(transform);
        }

        var dogConcert = FindObjectOfType<DogConcertHideShow>();
        var toConcert = dogConcert.transform.position - transform.position;
        toConcert.y = 0;
        if (moveHiddenPositions)
        {
            targetHidden.position = transform.position;
            targetHidden.rotation = transform.rotation;
            targetHidden.position = transform.position - toConcert.normalized * 10f;
        }

        targetAtConcert.rotation = Quaternion.LookRotation(toConcert);
    }

    private void OnEnable()
    {
        targetAtConcert.SetParent(transform.parent, true);
        targetHidden.SetParent(transform.parent, true);
        if (toggleRandomDogOnEnable)
        {
            ToggleRandomDog();
        }

    }

    public void RotateOnSpot()
    {
        // init in the right direction
        var targetPos = isAtConcert ? targetAtConcert.position : targetHidden.position;
        var targetRot = isAtConcert ? targetAtConcert.rotation : targetHidden.rotation;
        RotateToTarget_Cor(targetPos, targetRot);
    }

    void Update()
    {
        Update_Locomotion();
    }

    private void Update_Locomotion()
    {
        var targetPos = isAtConcert ? targetAtConcert.position : targetHidden.position;

        var distToTarget = Vector3.Distance(transform.position, targetPos);
        // this starts the movement. stopping happens when we are within the small threshold.
        if (!isMoving)
        {
            if (distToTarget < thresholdForStartMoving)
            {
                return;
            }
            else
            {
                isMoving = true;
            }
        }

        var targetRot = isAtConcert ? targetAtConcert.rotation : targetHidden.rotation;

        // if we're close enough, lerp to zero and stop moving.
        if (distToTarget < thresholdForStopMoving)
        {
            isMoving = false;
            RotateToTarget_Cor(targetPos, targetRot);
            return;
        }
        else
        {
            var walkOffset = Vector3.up * Mathf.Sin(Time.time * steppingSin) * stepHeight;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime) + walkOffset;

            var movementDirection = (targetPos - transform.position);
            if (movementDirection.sqrMagnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(movementDirection);
            }
            // var rotOffset = Quaternion.Euler(Mathf.Sin(Time.time * steppingRotSin) * 5f, Mathf.Sin(Time.time * steppingRotSin + 0.124351f) * 5f, 0);
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 360f * Time.deltaTime)
            //     * rotOffset;
        }

    }

    private void RotateToTarget_Cor(Vector3 targetPos, Quaternion targetRot)
    {
        StartCoroutine(pTween.To(0.3f, t =>
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.1f);
        }));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetAtConcert.position, 0.1f);
        Gizmos.DrawLine(targetAtConcert.position, targetHidden.position);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetHidden.position, 0.1f);
    }
}