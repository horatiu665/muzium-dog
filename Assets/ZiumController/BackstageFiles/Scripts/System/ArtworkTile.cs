using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtworkTile : MonoBehaviour
{
    public bool unlocked;
    [SerializeField] private string associatedArtworkName;
    private GameObject associatedArtworkObject;
    private ArtworkTriggerBox associatedArtworkScript;
    private Button button;
    private Image img;
    [SerializeField] private int rank;
    // Start is called before the first frame update
    void Start()
    {
        if (!GameObject.Find(associatedArtworkName)) {
            this.enabled = false;
            return;
        }

        associatedArtworkObject = GameObject.Find(associatedArtworkName);
        associatedArtworkScript = associatedArtworkObject.GetComponentInChildren<ArtworkTriggerBox>();
        button = GetComponent<Button>();
        img = GetComponentInChildren<Image>();

    }

    bool unlockApplication = true;

    // Update is called once per frame
    void Update()
    {
        unlocked = associatedArtworkScript.unlocked;
        button.interactable = unlocked;
        img.enabled = unlocked;

        if (unlocked && unlockApplication)
        {
            if (UserInterface.Instance.listCount < rank) UserInterface.Instance.listCount = rank;
            UserInterface.Instance.discoveredArtworks[rank] = this;
            unlockApplication = false;
        }
    }

    public void Make()
    {
        unlocked = associatedArtworkScript.unlocked;
        button.interactable = unlocked;
        img.enabled = unlocked;

        if (unlocked && unlockApplication)
        {

            UserInterface.Instance.listCount = rank;
            UserInterface.Instance.discoveredArtworks.Insert(rank, this);
            unlockApplication = false;
        }
    }

    public void OpenDetails()
    {
        UserInterface.Instance.artworkIndex = rank;
        transform.parent.parent.gameObject.SetActive(false);
        MuseumProgram.Instance.OpenArtworkDetails(gameObject.name, this);
    }

    public void OpenDetailsFromScript()
    {
        transform.parent.parent.gameObject.SetActive(false);
        MuseumProgram.Instance.OpenArtworkDetails(gameObject.name, this);
    }

    public void GiveNameToDetails(ArtworkDetails details)
    {
        details.associatedArtworkName = associatedArtworkName;
    }
}
