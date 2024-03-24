using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMouthBrain : MonoBehaviour
{
    [SerializeField]
    private DogRefs _dogRefs;
    public DogRefs dogRefs
    {
        get
        {
            if (_dogRefs == null)
            {
                _dogRefs = GetComponentInParent<DogRefs>();
            }
            return _dogRefs;
        }
    }

    public Animator anim => dogRefs.anim;

    public string animBarkTrigger = "Bark";
    public string animPantBool = "Pant";
    public string animMouthMildBool = "MouthMild";

    public enum MouthStates
    {
        None,
        Mild,
        Pant,
        Bark
    }

    public void Bark()
    {
        anim.SetTrigger(animBarkTrigger);
    }

    public void Pant(bool pant)
    {
        anim.SetBool(animPantBool, pant);
    }

    public void MildMouthMovement(bool active)
    {
        anim.SetBool(animMouthMildBool, active);
    }

    public void ResetMouth()
    {
        anim.ResetTrigger(animBarkTrigger);
        MildMouthMovement(false);
        Pant(false);
    }

}
