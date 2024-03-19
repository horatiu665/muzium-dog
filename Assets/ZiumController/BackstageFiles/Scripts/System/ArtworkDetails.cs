using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtworkDetails : MonoBehaviour
{
    [HideInInspector] public string associatedArtworkName;

    public void TeleportToArtwork()
    {
        MuseumProgram.Instance.TeleportToArtwork(associatedArtworkName);
        UserInterface.Instance.GoBack();
    }
}
