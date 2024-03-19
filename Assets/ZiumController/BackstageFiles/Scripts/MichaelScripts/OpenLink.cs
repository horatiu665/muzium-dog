using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public string Website;

    public void OpenChannel()
    {
        Application.OpenURL(Website);
    }
}
