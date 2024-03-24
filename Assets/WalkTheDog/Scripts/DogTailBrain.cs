using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogTailBrain : MonoBehaviour
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

    public Transform tail => dogRefs.tail;

    public float wagSpeed = 1;

    void Update()
    {
        tail.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.time * wagSpeed) * 30, 0);

    }

}
