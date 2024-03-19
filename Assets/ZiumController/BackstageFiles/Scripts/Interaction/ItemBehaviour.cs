using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : Interactable
{
    private Rigidbody rb;
    [HideInInspector] public bool inHands;
    public bool pickableObject = true;
    private bool isObserving = false;
    [HideInInspector] public Transform referenceTransform;
    
    private Transform cameraTransform;
    public float rotationSpeed = 5.0f;

    [Header("Observe Settings")]
    public Vector3 observeOffset = new Vector3(0, 0, 2); // Default offset
    public bool canZoomWhileLookedAt;

    // Rotation constraints
    public bool allowRotateX = true;
    public bool allowRotateY = true;
    public bool allowRotateZ = true;

    public override void Awake()
    {
        base.Awake();
        gameObject.layer = 7;
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    public override void OnInteract()
    {
        Debug.Log("OnInteract");
    }

    public override void OnFocus()
    {
        Debug.Log("OnFocus");
    }

    public override void OnLoseFocus()
    {
        Debug.Log("OnLoseFocus");
    }

    void Update()
    {
        if (inHands)
        {
            HandleInHands();
        }
        else
        {
            HandleOutOfHands();
        }
    }

    private void HandleInHands()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.interpolation = RigidbodyInterpolation.None;
        gameObject.layer = 0;

        if (Input.GetMouseButtonDown(1)) // Right mouse button clicked
        {
            EnterObserveMode();
        }

        if (isObserving)
        {
            if (Input.GetMouseButton(1)) // Right mouse button held down
            {
                RotateObject();
            }
            else if (Input.GetMouseButtonUp(1)) // Right mouse button released
            {
                ExitObserveMode();
            }
        }
    }

    private void HandleOutOfHands()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        gameObject.layer = 7;
    }

    private void EnterObserveMode()
    {
        // Apply the offset to the camera's position and then move forward by a specific distance
        transform.position = cameraTransform.position + cameraTransform.forward * 2 + observeOffset;
        isObserving = true;
        FirstPersonController.Instance.isObserving = true;
        FirstPersonController.Instance.canZoomWithItemInLook = canZoomWhileLookedAt;
    }

    private void RotateObject()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        if (allowRotateY)
        {
            transform.Rotate(cameraTransform.up, -mouseX, Space.World);
        }
        if (allowRotateX)
        {
            transform.Rotate(cameraTransform.right, mouseY, Space.World);
        }
        // Note: Rotation around Z-axis is less common in such mechanics but can be implemented similarly if needed
    }

    private void ExitObserveMode()
    {
        transform.position = referenceTransform.position;
        transform.rotation = referenceTransform.rotation;
        isObserving = false;
        FirstPersonController.Instance.isObserving = false;
    }

    public void ThrowItem(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
