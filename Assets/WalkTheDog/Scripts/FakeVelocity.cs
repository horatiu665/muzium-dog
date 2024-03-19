using UnityEngine;

public class FakeVelocity : MonoBehaviour
{
    [Header("Generated component. Can keep track of velocity of this object.")]

    public Vector3 velocity;
    public float speed;
    private Vector3 lastPosition;
    public float smoothness = 0.33f;

    void FixedUpdate()
    {
        var newVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        velocity = Vector3.Lerp(velocity, newVelocity, smoothness);
        lastPosition = transform.position;

        speed = velocity.magnitude;

    }
}