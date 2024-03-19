using UnityEngine;

public class ControlPanelToggle : MonoBehaviour
{

    public Collider collider;

    public GameObject dogStatusOn, dogStatusOff;

    public GameObject highlightOn;

    private void OnValidate()
    {
        if (collider == null)
        {
            collider = GetComponent<Collider>();
        }
    }

    public void SetUI(bool dogEnabled)
    {
        dogStatusOn.SetActive(dogEnabled);
        dogStatusOff.SetActive(!dogEnabled);
    }

    public void SetHighlight(bool highlight)
    {
        highlightOn.SetActive(highlight);
    }
}