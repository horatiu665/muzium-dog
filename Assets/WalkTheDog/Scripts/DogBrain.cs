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
                _dogRefs = GetComponentInParent<DogRefs>();
            }
            return _dogRefs;
        }
    }

    // player ref
    private Transform _player;
    public Transform player
    {
        get
        {
            if (_player != null)
                return _player;

            // find player. replace with code from ZIUM
            try
            {
                var cc = Camera.main.transform.GetComponentInParent<CharacterController>();
                if (cc != null)
                {
                    _player = cc.transform;
                    if (_player != null)
                    {
                        _playerFPC = _player.GetComponent<FirstPersonController>();
                    }
                }

                if (_player == null)
                {
                    _player = Camera.main.transform.GetComponentInParent<Rigidbody>().transform;
                }


            }
            catch (Exception e)
            {
                if (mainCamera != null)
                {
                    Debug.Log("No player found! Using main camera as player!");
                    _player = mainCamera.transform;
                }
                else
                {
                    Debug.LogError("No player found! Big problem for the dog!");
                    dummyPlayer = new GameObject("DummyPlayer");
                    _player = dummyPlayer.transform;
                }
            }

            return _player;
        }
    }

    // zium player
    private FirstPersonController _playerFPC;
    public FirstPersonController playerFPC
    {
        get
        {
            if (_playerFPC == null)
            {
                _playerFPC = player.GetComponent<FirstPersonController>();
            }
            return _playerFPC;
        }
    }

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

    public DogSniffingBrain dogSniffingBrain;

    public DogBarkingBrain dogBarkingBrain;

    public DogPettingBrain dogPettingBrain;

    public DogEmotionBrain dogEmotionBrain;
    public DogMouthBrain dogMouthBrain;
    public DogTailBrain dogTailBrain;

    public DogVoice dogVoice;

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

    }

    public bool IsThisThePlayer(Collider collider)
    {
        return collider.transform == player;
    }
}