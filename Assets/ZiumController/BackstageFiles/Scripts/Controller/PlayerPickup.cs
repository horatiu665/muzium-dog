using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private float detectRaySize = 5f;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private Transform itemHolder;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask plaqueLayer;
    [SerializeField] private float plaqueTextFadeSpeed = 5f;
    [SerializeField] TextMeshProUGUI messageText;
    public GameObject objInHand;
    private ItemBehaviour lastInteractable;
    private ItemBehaviour currentInteractable;
    RaycastHit hit;
    public bool detectItem;
    private Camera playerCamera;
    private Transform cam;
    private FirstPersonController player;

    private bool canInteract = true;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        player = GetComponent<FirstPersonController>();
        cam = playerCamera.transform;
        messageText = GameObject.Find("PlaqueText").GetComponent<TextMeshProUGUI>();
        messageText.gameObject.SetActive(false);
    }

    bool hasDetectedItem;

    void Update()
    {
        if (!canInteract) return;

        detectItem = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 5, itemLayer);
        currentInteractable = (detectItem ? hit.transform.GetComponent<ItemBehaviour>() : null);


        if (!detectItem) hasDetectedItem = true;

        if (detectItem && hasDetectedItem)
        {
            hasDetectedItem = false;
            currentInteractable.OnFocus();
        }
        else if (currentInteractable != lastInteractable && lastInteractable != null)
        {
            lastInteractable.OnLoseFocus();
        }

        lastInteractable = currentInteractable;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (detectItem && !player.isObserving)
            {
                currentInteractable.OnInteract();

                if (!currentInteractable.pickableObject) return;

                if (objInHand != null) 
                {
                    objInHand.GetComponent<ItemBehaviour>().inHands = false;
                    objInHand.transform.SetParent(null);
                    objInHand.GetComponent<ItemBehaviour>().ThrowItem(playerCamera.transform, throwForce);
                    objInHand = null;
                    objInHand = hit.transform.gameObject;
                    objInHand.GetComponent<ItemBehaviour>().inHands = true;
                    objInHand.transform.position = itemHolder.position + objInHand.GetComponent<ItemBehaviour>().positionOffset;
                    objInHand.transform.rotation = itemHolder.rotation;
                    objInHand.GetComponent<ItemBehaviour>().referenceTransform = itemHolder;
                    objInHand.transform.rotation = itemHolder.rotation;
                    objInHand.transform.SetParent(itemHolder);
                }
                else{
                    objInHand = hit.transform.gameObject;
                    objInHand.GetComponent<ItemBehaviour>().inHands = true;
                    objInHand.transform.position = itemHolder.position + objInHand.GetComponent<ItemBehaviour>().positionOffset;
                    objInHand.transform.rotation = itemHolder.rotation;
                    objInHand.GetComponent<ItemBehaviour>().referenceTransform = itemHolder;
                    objInHand.transform.SetParent(itemHolder);
                }
            }
            else if (objInHand != null && !player.isObserving) 
            {
                objInHand.GetComponent<ItemBehaviour>().inHands = false;
                objInHand.transform.SetParent(null);
                objInHand.GetComponent<ItemBehaviour>().ThrowItem(playerCamera.transform, throwForce);
                objInHand = null;
            }
        }
        HandlePlaqueSystem();
    }

    void HandlePlaqueSystem()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 6, plaqueLayer))
        {

            messageText.gameObject.SetActive(true);
            messageText.text = hit.transform.GetComponent<PlaqueScript>().message;
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, Mathf.Lerp(messageText.color.a, 1, plaqueTextFadeSpeed * Time.deltaTime));
            

        }
        else
        {
            Color tmpColor = messageText.color;
            messageText.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(messageText.color.a, 0, plaqueTextFadeSpeed * Time.deltaTime));
            if (messageText.color.a < 0.05f) messageText.gameObject.SetActive(false);
        }
    }

    public void StopInteracting() {
        objInHand.GetComponent<ItemBehaviour>().inHands = false;
        objInHand.GetComponent<ItemBehaviour>().ThrowItem(playerCamera.transform, throwForce);
        objInHand.transform.SetParent(null);
        objInHand = null;
    }


}
