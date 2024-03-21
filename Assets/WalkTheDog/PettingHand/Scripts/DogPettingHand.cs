using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class DogPettingHand : MonoBehaviour
{
    private static DogPettingHand _instance;
    public static DogPettingHand instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DogPettingHand>();
            }
            return _instance;
        }
    }

    // should be under the main camera
    public Camera mainCamera => Camera.main;

    public GameObject handVisual;

    public Transform handTransform;

    public Transform handHiddenPosition;
    public Transform handReadyPosition;

    public enum HandPettiness
    {
        Hidden,
        Ready,
        Petting
    }
    public HandPettiness handPettiness = HandPettiness.Hidden;
    private HandPettiness prevHandPettiness;
    private float _handHiddenTime;

    public Vector3 pettingTargetPosition;
    public Quaternion pettingTargetRotation;

    public float smoothnes = 0.2f;

    [Range(0, 1f)]
    public float spherecastRadius = 0.2f;

    [Space]

    public bool ignorePickupable = true;

    private float _pettingTime;
    private float _prevPettingOffsetCurveValue;
    private bool _prevOffsetWasDescending;
    public float pettingOffsetAmount = 0.2f;
    public AnimationCurve pettingOffsetCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 1),
        new Keyframe(1, 0)
        );

    public event System.Action<PettableObject> OnPettingStart, OnPettingEnd;
    private PettableObject _currentPettableObject;

    public AudioClip pettingClip
    {
        get
        {
            if (_currentPettableObject != null && _currentPettableObject.customPettingClips.Length > 0)
            {
                return _currentPettableObject.customPettingClips[Random.Range(0, _currentPettableObject.customPettingClips.Length)];
            }
            return defaultPettingClips[Random.Range(0, defaultPettingClips.Length)];
        }
    }

    public AudioClip[] defaultPettingClips;

    public SmartSoundDog pettingSound;

    public void PetThis(Transform petTarget)
    {
        PetThis(petTarget.position, petTarget.rotation);
    }

    public void PetThis(Vector3 petTargetPosition, Quaternion petTargetRotation)
    {
        handPettiness = HandPettiness.Petting;
        this.pettingTargetPosition = petTargetPosition;
        this.pettingTargetRotation = petTargetRotation;
    }

    public void StopPetting()
    {
        handPettiness = HandPettiness.Ready;
    }

    void OnEnable()
    {
        prevHandPettiness = handPettiness;

        // parent this root object under the main camera, to make use of local position
        transform.SetParent(mainCamera.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;


    }

    void Update()
    {
        if (DogControlPanel.instance == null || !DogControlPanel.instance.dogEnabled)
        {
            handVisual.SetActive(false);
            return;
        }

        // on change pettiness
        if (handPettiness != prevHandPettiness)
        {
            prevHandPettiness = handPettiness;
            if (handPettiness == HandPettiness.Hidden)
            {
                pettingTargetPosition = handHiddenPosition.position;
                pettingTargetRotation = handHiddenPosition.rotation;
                _handHiddenTime = Time.time;
                handVisual.transform.SetParent(mainCamera.transform, true);
            }
            else if (handPettiness == HandPettiness.Ready)
            {
                handVisual.SetActive(true);
                handVisual.transform.SetParent(mainCamera.transform, true);
                pettingTargetPosition = handReadyPosition.position;
                pettingTargetRotation = handReadyPosition.rotation;
            }
            else if (handPettiness == HandPettiness.Petting)
            {
                handVisual.transform.SetParent(null);
                handVisual.SetActive(true);
                _pettingTime = 0f;
            }
        }

        // on update pettiness
        {
            if (handPettiness == HandPettiness.Hidden)
            {
                pettingTargetPosition = handHiddenPosition.position;
                pettingTargetRotation = handHiddenPosition.rotation;
                if (Time.time > _handHiddenTime + 1f)
                {
                    handVisual.SetActive(false);
                }
            }
            else if (handPettiness == HandPettiness.Ready)
            {
                pettingTargetPosition = handReadyPosition.position;
                pettingTargetRotation = handReadyPosition.rotation;
            }
            else if (handPettiness == HandPettiness.Petting)
            {


            }
        }

        // lerp hand to petting target
        {
            _pettingTime += Time.deltaTime;
            var pettingOffsetCurveValue = pettingOffsetCurve.Evaluate(_pettingTime);
            var offsetPosition = handTransform.up * pettingOffsetCurveValue * pettingOffsetAmount;
            // var offsetRotation = Quaternion.identity;
            if (handPettiness != HandPettiness.Petting)
            {
                offsetPosition = Vector3.zero;
            }
            else
            {
                // when the offset hits zero / changes direction upwards, play sound
                var isDescending = _prevPettingOffsetCurveValue > pettingOffsetCurveValue;
                if (isDescending && !_prevOffsetWasDescending)
                {
                    pettingSound.audio.clip = pettingClip;
                    pettingSound.Play();
                }
                _prevOffsetWasDescending = isDescending;
                _prevPettingOffsetCurveValue = pettingOffsetCurveValue;

            }

            if (handPettiness == HandPettiness.Petting)
            {
                handTransform.position = Vector3.Lerp(handTransform.position, pettingTargetPosition + offsetPosition, smoothnes);
                handTransform.rotation = Quaternion.Lerp(handTransform.rotation, pettingTargetRotation, smoothnes);
            }
            else 
            // if (handPettiness == HandPettiness.Ready)
            {
                handTransform.localPosition = Vector3.Lerp(handTransform.localPosition, handTransform.parent.InverseTransformPoint(pettingTargetPosition + offsetPosition), smoothnes);
                handTransform.rotation = Quaternion.Lerp(handTransform.rotation, pettingTargetRotation, smoothnes);
            }
        }

        // detect pettable object under hand, update handPettiness, do petting events.
        {
            handPettiness = HandPettiness.Hidden;

            RaycastHit hit;
            if (Physics.SphereCast(mainCamera.transform.position, spherecastRadius, mainCamera.transform.forward, out hit, 2f))
            {
                var isPettable = PettableObject.IsPettable(hit.collider, out var pettableObj);

                if (pettableObj != null && ignorePickupable)
                {
                    if (pettableObj.IsPickupableByPlayer())
                    {
                        isPettable = false;
                    }
                }


                if (pettableObj != null && isPettable)
                {
                    // hand should be ready
                    handPettiness = HandPettiness.Ready;

                    // if we have petting input, start petting
                    if (PettableInputPressed())
                    {
                        if (_currentPettableObject == null)
                        {
                            _currentPettableObject = pettableObj;
                            _currentPettableObject.TriggerPettingStart();
                            OnPettingStart?.Invoke(pettableObj);
                        }
                        else
                        {
                            // change current pettable object
                            if (_currentPettableObject != pettableObj)
                            {
                                _currentPettableObject.TriggerPettingEnd();
                                OnPettingEnd?.Invoke(_currentPettableObject);
                                _currentPettableObject = pettableObj;
                                _currentPettableObject.TriggerPettingStart();
                                OnPettingStart?.Invoke(pettableObj);
                            }
                        }

                        (var petPosition, var petRotation) = pettableObj.GetPetLocation(handReadyPosition);

                        PetThis(petPosition, petRotation);
                    }
                }
            }

            if (handPettiness == HandPettiness.Hidden || handPettiness == HandPettiness.Ready)
            {
                // end petting.
                if (_currentPettableObject != null)
                {
                    _currentPettableObject.TriggerPettingEnd();
                    OnPettingEnd?.Invoke(_currentPettableObject);
                    _currentPettableObject = null;
                }
            }
        }

    }


    public bool PettableInputPressed()
    {
        // use the ZIUM method (if they change I'ma have to update)
        return Input.GetKey(KeyCode.E) || Input.GetMouseButton(0);

        // return Input.GetMouseButton(0);
    }
}


