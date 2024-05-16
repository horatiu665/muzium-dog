using System;
using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Events;

public class DogProximity : MonoBehaviour
{
    [Serializable]
    public class AreaData
    {
        public string areaName;
        public float areaRadius;
        public bool isPlayerInside;
        public bool isDogInside;
    }

    public List<AreaData> areas = new List<AreaData>();
    public AreaData currentPlayerArea;
    public AreaData currentDogArea;

    public event System.Action<AreaData> OnPlayerEnterArea;
    public event System.Action<AreaData> OnPlayerExitArea;

    public event System.Action<AreaData> OnDogEnterArea;
    public event System.Action<AreaData> OnDogExitArea;

    private void Reset()
    {
        Editor_InitAreas();
    }

    [DebugButton]
    public void Editor_InitAreas()
    {
        areas.Clear();
        areas.AddRange(new List<AreaData>{
        new AreaData()
        {
            areaName = "CLOSE",
            areaRadius = 3,
        }, new AreaData()
        {
            areaName = "MID",
            areaRadius = 6,
        }, new AreaData()
        {
            areaName = "FAR",
            areaRadius = 10,
        }});
    }

    private void OnEnable()
    {
        areas.Sort((a, b) => a.areaRadius.CompareTo(b.areaRadius));
    }

    private void Update()
    {
        if (!DogCastleReferences.instance.dogControlPanel.dogEnabled)
        {
            // exit all areas and return
            if (currentDogArea != null)
            {
                OnDogExitArea?.Invoke(currentDogArea);
                currentDogArea = null;
            }
            if (currentPlayerArea != null)
            {
                OnPlayerExitArea?.Invoke(currentPlayerArea);
                currentPlayerArea = null;
            }
            return;
        }

        var playerPos = DogCastleReferences.instance.dogBrain.player.transform.position;
        var playerDist = Vector3.Distance(playerPos, transform.position);
        var dogPos = DogCastleReferences.instance.dogBrain.transform.position;
        var dogDist = Vector3.Distance(dogPos, transform.position);
        var oldPlayerArea = currentPlayerArea;
        var oldDogArea = currentDogArea;
        currentPlayerArea = null; // if too far from any area, it's null.
        currentDogArea = null;
        for (int i = 0; i < areas.Count; i++)
        {
            var area = areas[i];
            if (playerDist < area.areaRadius)
            {
                currentPlayerArea = area;
                break;
            }
        }
        for (int i = 0; i < areas.Count; i++)
        {
            var area = areas[i];
            if (dogDist < area.areaRadius)
            {
                currentDogArea = area;
                break;
            }
        }

        // changed player area
        if (currentPlayerArea != oldPlayerArea)
        {
            if (oldPlayerArea != null)
            {
                oldPlayerArea.isPlayerInside = false;
                OnPlayerExitArea?.Invoke(oldPlayerArea);
            }
            if (currentPlayerArea != null)
            {
                currentPlayerArea.isPlayerInside = true;
                OnPlayerEnterArea?.Invoke(currentPlayerArea);
            }
        }

        // changed dog area
        if (currentDogArea != oldDogArea)
        {
            if (oldDogArea != null)
            {
                oldDogArea.isDogInside = false;
                OnDogExitArea?.Invoke(oldDogArea);
            }
            if (currentDogArea != null)
            {
                currentDogArea.isDogInside = true;
                OnDogEnterArea?.Invoke(currentDogArea);
            }
        }

    }

    private void OnDrawGizmos()
    {
        foreach (var area in areas)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, area.areaRadius);
        }
    }

}
