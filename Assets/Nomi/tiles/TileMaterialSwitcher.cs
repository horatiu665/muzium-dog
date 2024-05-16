using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class TileMaterialSwitcher : MonoBehaviour
{
    [SerializeField]
    private Renderer _r;
    public Renderer r
    {
        get
        {
            if (_r == null)
            {
                _r = GetComponentInChildren<Renderer>();
            }

            return _r;
        }
    }


    public List<Material> materials = new();

    public void ToggleRandom()
    {
        var randomIndex = Random.Range(0, materials.Count);
        SwitchTo(randomIndex);
    }

    [DebugButton]
    public void SwitchTo(int index)
    {
        if (index < 0 || index >= materials.Count)
        {
            Debug.LogError("Invalid index: " + index);
            return;
        }

        if (r == null)
        {
            Debug.LogError("No renderer found on " + name);
            return;
        }

        r.sharedMaterial = materials[index];

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(r);
#endif
    }

    [DebugButton]
    public void SwitchToNext()
    {
        if (r == null)
        {
            Debug.LogError("No renderer found on " + name);
            return;
        }

        var currentMaterial = r.sharedMaterial;
        var currentIndex = materials.IndexOf(currentMaterial);
        if (currentIndex == -1)
        {
            // not found. do nothing, just sets the first one.
        }

        var nextIndex = (currentIndex + 1) % materials.Count;
        r.sharedMaterial = materials[nextIndex];
    }
}
