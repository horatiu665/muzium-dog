using PlantmanAI4;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPrioritiesDebug : MonoBehaviour
{
    public List<Component> statesToDebug = new List<Component>();

    public List<Transform> scaleTheseOnYToPreviewPriorities = new List<Transform>();

    private void Update()
    {
        var n = Mathf.Min(statesToDebug.Count, scaleTheseOnYToPreviewPriorities.Count);
        for (int i = 0; i < n; i++)
        {
            var s = statesToDebug[i];
            if (!(s is IState))
            {
                s = s.GetComponent<IState>() as Component;
            }

            if (s is IState)
            {
                var d = scaleTheseOnYToPreviewPriorities[i];
                var priority = (s as IState).GetPriority();

                d.localScale = new Vector3(0.01f, priority, 0.01f);
                d.localPosition = new Vector3(d.localPosition.x, priority * 0.5f, d.localPosition.z);
            }
        }

    }


}
