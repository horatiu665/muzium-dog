using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceNearestToLineSegment : MonoBehaviour
{
    public Transform A, B;

    private Transform _listener;
    public Transform listener
    {
        get
        {
            if (_listener == null)
            {
                _listener = Camera.main.transform;
            }
            return _listener;
        }
    }
    
    private void Update()
    {
        // move the audio source to the nearest point on the line segment A-B, clamped between A and B
        Vector3 nearestPoint = Vector3.Lerp(A.position, B.position, Mathf.Clamp01(Vector3.Dot(listener.position - A.position, B.position - A.position) / Vector3.SqrMagnitude(B.position - A.position)));
        transform.position = nearestPoint;

    }
}
