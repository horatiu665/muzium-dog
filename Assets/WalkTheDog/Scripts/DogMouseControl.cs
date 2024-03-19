using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMouseControl : MonoBehaviour
{
    public Camera mainCamera => Camera.main;
    public DogLocomotion dogLocomotion;

    [Space]
    public bool doAstar = false;
    public DogAstar dogAstar;

    public bool useShiftForDirect = true;

    [Range(0, 1f)]
    public float clickTargetSpeed = 1f;
    private Vector3 clickTarget;
    private bool hasClickDestination;
    public bool HasClickDestination => hasClickDestination;

    void Update()
    {
        if (hasClickDestination)
        {
            if (Vector3.Distance(dogLocomotion.rbRoot.position, clickTarget) <= dogLocomotion.stopDistance)
            {
                hasClickDestination = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f, dogAstar.aStar.layerMask))
            {
                // for keeping track of click-locomotion-requests
                clickTarget = hit.point;
                hasClickDestination = true;

                dogLocomotion.targetSpeed01 = clickTargetSpeed;

                if (useShiftForDirect)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        dogLocomotion.SetDestination(hit.point);
                        return;
                    }
                }

                if (doAstar)
                {
                    dogAstar.SetDestination(hit.point);
                }
                else
                {
                    dogLocomotion.SetDestination(hit.point);
                    dogLocomotion.SetTargetRotation(hit.point - dogLocomotion.rbRoot.position);
                }
            }
        }
        // stop. 1 is right click
        if (Input.GetMouseButtonDown(1))
        {
            hasClickDestination = false;

            if (doAstar)
            {
                dogAstar.StopMovement();
            }
            else
            {
                dogLocomotion.StopMovement();
                dogLocomotion.StopRotation();
            }
        }
    }
}
