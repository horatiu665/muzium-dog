using UnityEngine;

public class Candle : MonoBehaviour
{
    public bool isOn;
    public GameObject fire;

    public int artworkId = 0;

    private void OnEnable()
    {
        Set(isOn);
    }

    public void Set(bool on)
    {
        isOn = on;
        fire.SetActive(on);
        // todo: animate if it changes
        
    }

}