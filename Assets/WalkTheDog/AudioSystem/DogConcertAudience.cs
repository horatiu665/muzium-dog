using System;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class DogConcertAudience : MonoBehaviour
{
    public AudienceAstar audienceAstar;

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

    private void Start()
    {
        if (isAtConcert)
        {
            transform.position = targetAtConcert.position;
            RotateOnSpot();
        }
        else
        {
            transform.position = targetHidden.position;
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
        var finalPos = isAtConcert ? targetAtConcert.position : targetHidden.position;
        var finalRot = isAtConcert ? targetAtConcert.rotation : targetHidden.rotation;

        var distToTarget = Vector3.Distance(transform.position, finalPos);
        // this starts the movement. stopping happens when we are within the small threshold.
        if (!isMoving)
        {
            if (distToTarget < thresholdForStartMoving)
            {
                currentPath = null;
                return;
            }
            else
            {
                isMoving = true;
            }
        }

        // if we're close enough, lerp to zero and stop moving.
        if (distToTarget < thresholdForStopMoving)
        {
            currentPath = null;
            isMoving = false;
            RotateToTarget_Cor(finalPos, finalRot);
            return;
        }
        else
        {
            WalkTowards(finalPos);
        }
    }

    private AStar.Path currentPath;

    [DebugButton]
    public void ClearPath()
    {
        currentPath = null;
    }

    private void WalkTowards(Vector3 finalPos)
    {
        if (true)
        // new method - using astar
        {
            if (currentPath == null)
            {
                // only make a path if we are far from the target
                // var distToTarget = Vector3.Distance(transform.position, finalPos);
                // if (distToTarget >= thresholdForStartMoving)
                {
                    Debug.Log("Made a path", this);
                    currentPath = new AStar.Path();
                    var startNode = audienceAstar.aStar.GetNearestNode(transform.position);
                    currentPath.startNode = startNode;
                    var endNode = audienceAstar.aStar.GetNearestNode(finalPos);
                    currentPath.endNode = endNode;

                    var nodes = audienceAstar.aStar.GetPath(currentPath.startNode, endNode);
                    if (nodes != null)
                    {
                        currentPath.nodes = nodes;
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            Debug.DrawLine(nodes[i].position, nodes[(i + 1) % nodes.Count].position, Color.green, 30);
                        }
                    }
                }
            }
            else
            {
                if (currentPath.nodes.Count > 1)
                {
                    var nextNode = currentPath.nodes[0];
                    var nextPos = nextNode.position;
                    var walkOffset = Vector3.up * Mathf.Sin(Time.time * steppingSin) * stepHeight;
                    transform.position = Vector3.MoveTowards(transform.position, nextPos, walkSpeed * Time.deltaTime) + walkOffset;

                    var movementDirection = (nextPos - transform.position);
                    if (movementDirection.sqrMagnitude > 0.1f)
                    {
                        transform.rotation = Quaternion.LookRotation(movementDirection);
                    }

                    if (Vector3.Distance(transform.position, nextPos) < thresholdForStopMoving)
                    {
                        currentPath.nodes.RemoveAt(0);
                    }
                }
                else if (currentPath.nodes.Count == 1)
                {
                    // move in straight line
                    var walkOffset = Vector3.up * Mathf.Sin(Time.time * steppingSin) * stepHeight;
                    transform.position = Vector3.MoveTowards(transform.position, finalPos, walkSpeed * Time.deltaTime) + walkOffset;

                    var movementDirection = (finalPos - transform.position);
                    if (movementDirection.sqrMagnitude > 0.1f)
                    {
                        transform.rotation = Quaternion.LookRotation(movementDirection);
                    }
                }
                else
                {
                    currentPath = null;
                }
            }

        }
        else
        // old method - direct line
        {
            var walkOffset = Vector3.up * Mathf.Sin(Time.time * steppingSin) * stepHeight;
            transform.position = Vector3.MoveTowards(transform.position, finalPos, walkSpeed * Time.deltaTime) + walkOffset;

            var movementDirection = (finalPos - transform.position);
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
        if (isActiveAndEnabled)
        {
            StartCoroutine(pTween.To(0.3f, t =>
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, t);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
            }));
        }

    }

    public bool drawGizmos = false;

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetAtConcert.position, 0.1f);
        Gizmos.DrawLine(targetAtConcert.position, targetHidden.position);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetHidden.position, 0.1f);

        // pathfinding debug
        Gizmos.color = currentPath == null ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 5, 1f);
        if (currentPath != null)
        {
            for (int i = 0; i < currentPath.nodes.Count; i++)
            {
                Gizmos.DrawWireSphere(transform.position + Vector3.up * (5 + i), 0.5f);
            }
        }
    }
}