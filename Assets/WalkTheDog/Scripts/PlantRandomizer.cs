using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class PlantRandomizer : MonoBehaviour
{
    public ToggleChildren plantToggler;
    public TileMaterialSwitcher tileToggler;

    public bool randomizeOnAwake = true;

    private void Awake()
    {
        if (randomizeOnAwake)
        {
            Randomize();
        }
    }

    public void Randomize()
    {
        plantToggler.ToggleRandom();
        tileToggler.ToggleRandom();

    }

}
