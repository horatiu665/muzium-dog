using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for the UI elements like Image

public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController Instance;

    public bool isSprinting;
    public bool isWalking; 
    public bool isGrounded;
    public bool isCrouching;
    public bool isZooming;
    public bool isObserving;
    public bool canZoomWithItemInLook;

    [Header("Controller settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private bool canMove;
    [SerializeField] private float gravityCst = 10;
    [SerializeField] private float jumpForce = 6;
    [SerializeField] private float acceleration = 10;
    [SerializeField] private float sprintAcceleration = 10;
    [SerializeField] private float deceleration = 20;
    [SerializeField] private float sprintDeceleration = 10;
    [SerializeField] private float pushRigidbodiesForce = 2;
    private Vector3 moveDirection;
    private float verticalForce;
    private float walkFactor;
    private float sprintFactor;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] footstepsConcreteClips;
    [SerializeField] private AudioClip[] footstepsSnowClips;
    [SerializeField] private AudioClip[] footstepsCarpetClips;
    [SerializeField] private AudioClip[] footstepsWoodClips;
    [SerializeField] private AudioClip fallOnConcrete;
    [SerializeField] private AudioClip zoomClip;
    [SerializeField] private AudioClip zoomOutClip;

    [Header("Camera")]
    private Camera playerCamera;
    [SerializeField] private float lookSpeedX = 2;
    [SerializeField] private float lookSpeedY = 2;
    [SerializeField] private float zoomSpeed = 10;
    [SerializeField] private float defaultFov = 65;
    [SerializeField] private float zoomDefaultFov = 40;
    [SerializeField] private float zoomScrollIncrement = 3;
    private float lookX;

    [Header("Reticle")]
    [SerializeField] public Image reticle; // Reticle UI element

    private CharacterController characterController;
    private PlayerPickup pickupScript;
    private bool shouldPlaySfx = true;
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        pickupScript = GetComponent<PlayerPickup>();
        reticle = GameObject.Find("Reticle").GetComponent<Image>();
        Instance = this;
    }

    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.transform.TryGetComponent<Rigidbody>(out Rigidbody rb) && characterController.isGrounded && collision.moveDirection.y > -0.3f)
        {
            rb.AddForce(transform.forward * pushRigidbodiesForce, ForceMode.Force);
        }
    }

    void Update()
    {
        if (!canMove || UserInterface.Instance.paused) return;

        isGrounded = characterController.isGrounded;
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        isWalking = axisInput.magnitude != 0;
        isZooming = Input.GetMouseButton(1) && (isObserving ? canZoomWithItemInLook : 1==1);

        HandleInput();
        HandleJump();
        HandleFootsteps();
        HandleMouseLook();
        HandleZooming();
        ApplyFinalMovements();
        HandleReticle(); // Call to handle the reticle visibility

        if (characterController.isGrounded) moveDirection.y = -2;
    }

    float xmov;
    float ymov;
    Vector2 axisInput;

    private void HandleInput()
    {
        if (isObserving) {
            moveDirection = new Vector3(0, moveDirection.y, 0);
            return;
        }
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        axisInput = new Vector2(x, y);

        if (x != 0) xmov = Mathf.Lerp(xmov, x, acceleration * Time.deltaTime);
        else xmov = Mathf.Lerp(xmov, 0, deceleration * Time.deltaTime);

        if (y != 0) ymov = Mathf.Lerp(ymov, y, acceleration * Time.deltaTime);
        else ymov = Mathf.Lerp(ymov, 0, deceleration * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift)) sprintFactor = Mathf.Lerp(sprintFactor, 1, sprintAcceleration * Time.deltaTime);
        else sprintFactor = Mathf.Lerp(sprintFactor, 0, sprintDeceleration * Time.deltaTime);

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.forward * ymov + transform.right * xmov) * (walkSpeed + (sprintSpeed - walkSpeed) * sprintFactor);

        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        if (isObserving) return;

        lookX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        lookX = Mathf.Clamp(lookX, -90, 90);
        playerCamera.transform.localRotation = Quaternion.Euler(lookX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleZooming()
    {
        if (Input.GetMouseButtonDown(1) && (isObserving ? canZoomWithItemInLook : 1==1)) SoundManager.Instance.PlayPlayerSound(zoomClip);
        if (Input.GetMouseButtonUp(1) && (isObserving ? canZoomWithItemInLook : 1==1)) SoundManager.Instance.PlayPlayerSound(zoomOutClip);

        var desiredFov = isZooming ? zoomDefaultFov : defaultFov;
        if (isZooming) zoomDefaultFov -= Input.mouseScrollDelta.y * zoomScrollIncrement;
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && isZooming) {
            SoundManager.Instance.PlayPlayerSound(zoomClip);
            UserInterface.Instance.DisplayFov(playerCamera.fieldOfView);
        }

        if (playerCamera.fieldOfView != desiredFov)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, desiredFov, zoomSpeed * Time.deltaTime);
        }

        zoomDefaultFov = Mathf.Clamp(zoomDefaultFov, 30, 115);
    }

    float coyoteTimer;
    float prejumpTimer;

    private void HandleJump()
    {   
        prejumpTimer -= Time.deltaTime;

        if (characterController.isGrounded) {
            if (coyoteTimer < 0) {
                SoundManager.Instance.PlayPlayerSound(fallOnConcrete);
                footstepTimer = GetCurrentOffset;
            }
            coyoteTimer = 0.2f;
        }
        else coyoteTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimer > 0 && moveDirection.y < 0)
        {
            moveDirection.y = jumpForce;
            ShuffleAudioClips(footstepsConcreteClips);
            SoundManager.Instance.PlayPlayerSound(footstepsConcreteClips[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            prejumpTimer = 0.25f;
        }
        else if (characterController.isGrounded && prejumpTimer > 0)
        {
            moveDirection.y = jumpForce;
            ShuffleAudioClips(footstepsConcreteClips);
            SoundManager.Instance.PlayPlayerSound(footstepsConcreteClips[0]);
        }
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded) moveDirection.y -= gravityCst * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    float footstepTimer;
    private float GetCurrentOffset => isCrouching ? 0.7f : isSprinting ? 0.33f : 0.55f;

    private void HandleFootsteps()
    {
        if (!characterController.isGrounded || axisInput.magnitude == 0 || !shouldPlaySfx) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            footstepTimer = GetCurrentOffset;

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3f))
            {
                switch(hit.collider.tag)
                {
                    case "Footsteps/Concrete":
                        ShuffleAudioClips(footstepsConcreteClips);
                        SoundManager.Instance.PlayPlayerSound(footstepsConcreteClips[0]);
                        break;
                    case "Footsteps/Snow":
                        ShuffleAudioClips(footstepsSnowClips);
                        SoundManager.Instance.PlayPlayerSound(footstepsSnowClips[0]);
                        break;
                    case "Footsteps/Wood":
                        ShuffleAudioClips(footstepsWoodClips);
                        SoundManager.Instance.PlayPlayerSound(footstepsWoodClips[0]);
                        break;
                    case "Footsteps/Carpet":
                        ShuffleAudioClips(footstepsCarpetClips);
                        SoundManager.Instance.PlayPlayerSound(footstepsCarpetClips[0]);
                        break;
                    default:
                        ShuffleAudioClips(footstepsConcreteClips);
                        SoundManager.Instance.PlayPlayerSound(footstepsConcreteClips[0]);
                        break;
                }
            }
        }
    }

    public void ShuffleAudioClips(AudioClip[] clips)
    {
        AudioClip firstClip = clips[0];

        // 1 to avoid repetition of one sound 
        int r = Random.Range(1, clips.Length);
        clips[0] = clips[r];
        clips[r] = firstClip;
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<NoPlayerSfxTrigger>())
        {
            shouldPlaySfx = false;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<NoPlayerSfxTrigger>())
        {
            shouldPlaySfx = true;
        }
    }

    private void HandleReticle()
    {
        if (isObserving)
            reticle.enabled = false;
        else
            reticle.enabled = pickupScript.detectItem;
    }

    private float originalWalkSpeed;
    private float originalSprintSpeed;

    public void FreezeMovement()
    {
        originalWalkSpeed = walkSpeed;
        originalSprintSpeed = sprintSpeed;

        walkSpeed = 0;
        sprintSpeed = 0;
    }

    public void UnfreezeMovement()
    {
        walkSpeed = originalWalkSpeed;
        sprintSpeed = originalSprintSpeed;
    }



}
