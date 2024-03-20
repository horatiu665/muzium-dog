using System;
using UnityEngine;

public class DogBrain : MonoBehaviour
{

    [SerializeField]
    private DogRefs _dogRefs;
    public DogRefs dogRefs
    {
        get
        {
            if (_dogRefs == null)
            {
                _dogRefs = GetComponent<DogRefs>();
            }
            return _dogRefs;
        }
    }

    // player ref
    public Transform player;

    // zium player
    public FirstPersonController playerFPC;

    private Camera _mainCamera;
    public Camera mainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            if (_mainCamera == null)
            {
                _mainCamera = FindObjectOfType<Camera>();
            }
            return _mainCamera;
        }
    }

    /// <summary>
    /// smart pathfinding movement
    /// </summary>
    public DogAstar dogAstar;

    /// <summary>
    /// DIRECT movement
    /// </summary>
    public DogLocomotion dogLocomotion => dogRefs.dogLocomotion;

    public DogLookAttention dogLook;

    public DogSniffingBrain dogSniffableObservation;

    public DogPettingBrain dogPettingBrain;

    public DogEmotionBrain dogEmotionBrain;

    private FakeVelocity _playerFakeVelocity;
    public FakeVelocity playerFakeVelocity
    {
        get
        {
            if (_playerFakeVelocity == null)
            {
                _playerFakeVelocity = player.GetComponent<FakeVelocity>();
                if (_playerFakeVelocity == null)
                {
                    _playerFakeVelocity = player.gameObject.AddComponent<FakeVelocity>();
                }
            }
            return _playerFakeVelocity;
        }
    }

    private GameObject dummyPlayer;

    private void Awake()
    {
        // find player. replace with code from ZIUM
        try
        {
            player = Camera.main.transform.GetComponentInParent<CharacterController>().transform;

            playerFPC = player.GetComponent<FirstPersonController>();

        }
        catch (Exception e)
        {
            if (mainCamera != null)
            {
                player = mainCamera.transform;
            }
            else
            {
                Debug.LogError("No player found! Big problem for the dog!");
                dummyPlayer = new GameObject("DummyPlayer");
                player = dummyPlayer.transform;
            }
        }
        // player = mainCamera.transform;

    }

    public bool IsThisThePlayer(Collider collider)
    {
        return collider.transform == player;
    }
}