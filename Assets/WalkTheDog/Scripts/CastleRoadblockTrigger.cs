using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleRoadblockTrigger : MonoBehaviour
{
    public event System.Action<Collider> OnTriggerEnterTriggered;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterTriggered?.Invoke(other);
    }

    
}
