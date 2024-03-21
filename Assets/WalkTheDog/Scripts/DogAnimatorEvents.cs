using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimatorEvents : MonoBehaviour
{
    // catch events from the animator
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

    // footsteps...
    public void Footstep()
    {
        dogRefs.dogBrain.dogVoice.Footstep();
    }

}
