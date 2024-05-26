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
        var sqrDist = (B.position - A.position).sqrMagnitude;
        if (float.IsNaN(sqrDist) || sqrDist == 0)
            return;

        var factorrr = Mathf.Clamp01(Vector3.Dot(listener.position - A.position, B.position - A.position)
             / sqrDist);
        if (float.IsNaN(factorrr))
            return;

        // move the audio source to the nearest point on the line segment A-B, clamped between A and B
        Vector3 nearestPoint = Vector3.Lerp(A.position, B.position, factorrr);
        transform.position = nearestPoint;

    }
}
