using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimator : MonoBehaviour
{
    private DogRefs _dogRefs;
    public DogRefs dogRefs
    {
        get
        {
            if (_dogRefs == null)
            {
                _dogRefs = GetComponent<DogRefs>();
            }
            return _dogRefs;
        }
    }

    private DogLocomotion _dogLocomotion;
    public DogLocomotion dogLocomotion
    {
        get
        {
            if (_dogLocomotion == null)
            {
                _dogLocomotion = GetComponent<DogLocomotion>();
            }
            return _dogLocomotion;
        }
    }
    public Animator anim => dogRefs.anim;
    public string animWalkSpeed = "WalkSpeed";


    private void Update()
    {
        if (dogRefs.dogBrain.dogBarkingBrain.isBarking)
        {
            anim.SetFloat(animWalkSpeed, 0);

        }
        else
        {
            anim.SetFloat(animWalkSpeed, dogLocomotion.currentSpeed01);

        }

    }


}

