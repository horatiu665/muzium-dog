using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogControlPanel : MonoBehaviour
{
    private static DogControlPanel _instance;
    public static DogControlPanel instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DogControlPanel>();
            }
            return _instance;
        }
    }
    
    public DogRefs dog;

    public bool startDogEnabled = true;
    public bool dogEnabled { get; private set;}

    public ControlPanelToggle c_toggleDogEnabled;

    public Transform dogStartPosition;


    // API to set dog status.
    public void SetDogEnabled(bool dogEnabled)
    {
        this.dogEnabled = dogEnabled;
        dog.gameObject.SetActive(dogEnabled);
        dog.transform.position = dogStartPosition.position;
        dog.transform.rotation = dogStartPosition.rotation;

        c_toggleDogEnabled.SetUI(dogEnabled);
    }


    // unity events.

    private void OnEnable()
    {
        // start disabled...
        SetDogEnabled(startDogEnabled);
    }

    private void Update()
    {
        // consider highlighting the control panel buttons when the player raycasts in front of them. 
        // that's more of an interaction system thing for the main game tho.
        c_toggleDogEnabled.SetHighlight(false);

        // raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == c_toggleDogEnabled.collider)
            {
                // highlight?
                c_toggleDogEnabled.SetHighlight(true);

                // consider enabling/disabling the control panel when the player is nearby
                if (Input.GetMouseButtonDown(0))
                {
                    // toggle dog enabled
                    dogEnabled = !dogEnabled;
                    SetDogEnabled(dogEnabled);
                }
            }

        }
    }

}
