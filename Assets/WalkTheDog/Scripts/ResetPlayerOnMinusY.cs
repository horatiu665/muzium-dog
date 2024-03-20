using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerOnMinusY : MonoBehaviour
{
    private Vector3 initPos;
    public float threshold = -50;
    public bool alsoRb = true;

    void Start()
    {
        initPos = transform.position;
    }

    void Update()
    {
        if (transform.position.y < threshold)
        {
            transform.position = initPos;
            var cc = GetComponent<CharacterController>();
            if (cc!=null){
                Physics.SyncTransforms();
            }
            if (alsoRb)
            {
                var rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }
}
