using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MuseumProgram : MonoBehaviour
{
    public static MuseumProgram Instance;
    [SerializeField] private Transform secondLayout;

    void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    void Update()
    {
        
    }

    public void TeleportToArtwork(string id)
    {
        UserInterface.Instance.ResumeGame();
        FirstPersonController.Instance.transform.position = GameObject.Find(id).transform.position;
        FirstPersonController.Instance.transform.forward = GameObject.Find(id).transform.forward;
        Physics.SyncTransforms();
    }

    public void OpenArtworkDetails(string name, ArtworkTile tile)
    {
        secondLayout.Find(name).gameObject.SetActive(true);
        tile.GiveNameToDetails(secondLayout.Find(name).GetComponent<ArtworkDetails>());
    }
}
