using ToyBoxHHH;
using UnityEngine;

public class Candle : MonoBehaviour
{
    public bool isOn;
    public GameObject fire;

    public int artworkId = 0;

    public ParticleSystem smoke;


    private void OnEnable()
    {
        Set(isOn);
    }

[DebugButton]
    public void Set(bool on)
    {
        bool changed = isOn != on;
        isOn = on;
        fire.SetActive(on);
        // todo: animate if it changes

        if (changed)
        {
            if (!on)
            {
                // smoke up.
                smoke.Play();
            }
            else {
                smoke.Stop();
            }
        }


    }

}