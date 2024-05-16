using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class TileMaterialSwitcher : MonoBehaviour
{
    public Renderer r;

    public List<Material> materials = new();

    [DebugButton]
    public void Editor_SwitchTo(int index)
    {
        if (index < 0 || index >= materials.Count)
        {
            Debug.LogError("Invalid index: " + index);
            return;
        }

        r = GetComponentInChildren<Renderer>();
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
    public void Editor_SwitchToNext()
    {
        r = GetComponentInChildren<Renderer>();
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
