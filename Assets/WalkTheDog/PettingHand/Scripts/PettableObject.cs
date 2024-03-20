using System;
using System.Collections.Generic;
using ToyBoxHHH;
using ToyBoxHHH.ReadOnlyUtil;
using UnityEngine;

public class PettableObject : MonoBehaviour
{
    public static List<PettableObject> allPettableObjects = new List<PettableObject>();

    public bool isDog { get; private set; }

    [Header("If set, chooses the nearest one to the petting hand and pets there. Rotation is automatic.")]
    public Transform[] optionalPettingPositions;

    [Header("If no pet locations are set, it uses the nearest collider.ClosestPoint(hand.position) to the camera. Rotation is automatic.")]
    public bool computePetPositionUsingCollider = true;
    public Collider[] pettingColliders;

    [Header("Otherwise, it uses the transform.position as petting location. Rotation is automatic.")]
    [ReadOnly]
    public bool pettingFallbackToTransform = true;

    public event Action OnPettingStart, OnPettingEnd;

    public AudioClip[] customPettingClips;

    // only called by DogPettingHand
    public void TriggerPettingStart()
    {
        OnPettingStart?.Invoke();
    }

    // only called by DogPettingHand
    public void TriggerPettingEnd()
    {
        OnPettingEnd?.Invoke();
    }

    [DebugButton]
    private void GetAllCollidersInChildren()
    {
        pettingColliders = GetComponentsInChildren<Collider>();

    }

    private void OnEnable()
    {
        isDog = GetComponentInParent<DogRefs>() != null;

        allPettableObjects.Add(this);
    }

    private void OnDisable()
    {
        allPettableObjects.Remove(this);
    }

    public static bool IsPettable(Collider collider, out PettableObject pettableObj)
    {
        pettableObj = collider.GetComponentInParent<PettableObject>();
        return pettableObj != null;
    }

    public (Vector3 petPosition, Quaternion petRotation) GetPetLocation(Transform petHandReadyLocation)
    {
        if (optionalPettingPositions.Length > 0)
        {
            Transform nearest = optionalPettingPositions[0];
            float nearestDist = Vector3.Distance(nearest.position, petHandReadyLocation.position);
            foreach (var loc in optionalPettingPositions)
            {
                float dist = Vector3.Distance(loc.position, petHandReadyLocation.position);
                if (dist < nearestDist)
                {
                    nearest = loc;
                    nearestDist = dist;
                }
            }

            var pettingSurfaceNormal = nearest.up;
            var handForward = petHandReadyLocation.forward;
            var handRight = petHandReadyLocation.right;
            var finalHandRotation = Quaternion.LookRotation(handForward, pettingSurfaceNormal);

            return (nearest.position, finalHandRotation);
        }
        else if (computePetPositionUsingCollider && pettingColliders.Length > 0)
        {
            Vector3 closestPoint = pettingColliders[0].ClosestPoint(petHandReadyLocation.position);
            float nearestDist = Vector3.Distance(closestPoint, petHandReadyLocation.position);
            foreach (var col in pettingColliders)
            {
                Vector3 point = col.ClosestPoint(petHandReadyLocation.position);
                float dist = Vector3.Distance(point, petHandReadyLocation.position);
                if (dist < nearestDist)
                {
                    closestPoint = point;
                    nearestDist = dist;
                }
            }
            return (closestPoint, petHandReadyLocation.rotation);
        }

        return (transform.position, transform.rotation);
    }


    public bool IsPickupableByPlayer()
    {
        // TODO: implement! by checking for the component used by Zium.
        return false;
    }
}