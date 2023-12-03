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


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f, dogAstar.aStar.layerMask))
            {
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
