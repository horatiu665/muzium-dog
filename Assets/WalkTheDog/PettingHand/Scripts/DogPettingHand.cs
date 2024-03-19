using System.Collections;
using System.Collections.Generic;
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
    public float pettingOffsetAmount = 0.2f;
    public AnimationCurve pettingOffsetCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 1),
        new Keyframe(1, 0)
        );

    public event System.Action<PettableObject> OnPettingStart, OnPettingEnd;
    private PettableObject _currentPettableObject;

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
                if (Time.time > _handHiddenTime + 1f)
                {
                    handVisual.SetActive(false);
                }
            }
            else if (handPettiness == HandPettiness.Ready)
            {

            }
            else if (handPettiness == HandPettiness.Petting)
            {
                // petting animation???


            }
        }

        // lerp hand to petting target
        {
            _pettingTime += Time.deltaTime;
            var offsetPosition = handTransform.up * pettingOffsetCurve.Evaluate(_pettingTime) * pettingOffsetAmount;
            // var offsetRotation = Quaternion.identity;
            if (handPettiness != HandPettiness.Petting)
            {
                offsetPosition = Vector3.zero;
            }

            handTransform.position = Vector3.Lerp(handTransform.position, pettingTargetPosition + offsetPosition, smoothnes);
            handTransform.rotation = Quaternion.Lerp(handTransform.rotation, pettingTargetRotation, smoothnes);
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
        return Input.GetMouseButton(0);
    }
}


