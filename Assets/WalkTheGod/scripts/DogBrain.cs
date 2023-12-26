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

    public DogAstar dogAstar;
    public DogLocomotion dogLocomotion => dogRefs.dogLocomotion;

    public DogLookAttention dogLook;

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

    private void Awake()
    {
        // find player. replace with code from ZIUM
        player = Camera.main.transform.GetComponentInParent<Rigidbody>().transform;
        // player = mainCamera.transform;

    }

    public bool IsThisThePlayer(Collider collider)
    {
        return collider.transform == player;
    }
}