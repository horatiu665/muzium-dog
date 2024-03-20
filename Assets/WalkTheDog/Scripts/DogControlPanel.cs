using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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
    public bool dogEnabled { get; private set; }

    public ControlPanelToggle c_toggleDogEnabled;

    public Transform dogStartPosition;

    public PlayableDirector dogIntroTimeline;

    public Collider doNotAllowPlayerInsideHereIfDoorIsClosed;

    public Transform playerInFrontOfDoorPosition;


    // API to set dog status.
    public void SetDogEnabled(bool dogEnabled)
    {
        this.dogEnabled = dogEnabled;
        dog.gameObject.SetActive(dogEnabled);
        dog.transform.position = dogStartPosition.position;
        dog.transform.rotation = dogStartPosition.rotation;

        c_toggleDogEnabled.SetUI(dogEnabled);

        if (dogEnabled)
        {
            dogIntroTimeline.Play();
        }
    }


    // unity events.

    private void OnEnable()
    {
        // start disabled...
        SetDogEnabled(startDogEnabled);
    }

    private void Update()
    {
        // consider only doing this when within range of the thing (<5m??)

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

                    if (!dogEnabled)
                    {
                        if (IsInside(doNotAllowPlayerInsideHereIfDoorIsClosed, dog.dogBrain.mainCamera.transform.position))
                        {
                            // move player outside

                            // this doesn't work because of ZIUM controller.

                            var player = dog.dogBrain.player;
                            

                            player.position = playerInFrontOfDoorPosition.position;
                            player.rotation = playerInFrontOfDoorPosition.rotation;

                            // Physics.SyncTransforms();
                            
                            // StartCoroutine(pTween.WaitFrames(1, ()=>{

                            // player.position = playerInFrontOfDoorPosition.position;
                            // player.rotation = playerInFrontOfDoorPosition.rotation;

                            // Physics.SyncTransforms();
                            
                            // }));
                        }


                    }
                }
            }

        }
    }

    // adjusted from https://discussions.unity.com/t/check-if-position-is-inside-a-collider/12667/3
    public static bool IsInside(Collider c, Vector3 point)
    {
        Vector3 closest = c.ClosestPoint(point);
        return (closest - point).magnitude <= Mathf.Epsilon;
    }
}
