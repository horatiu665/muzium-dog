using System;
using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Events;

public class DogProximity : MonoBehaviour
{
    public Transform debugDawg;


    [Serializable]
    public class AreaData
    {
        public string areaName;
        public float areaRadius;
        public bool isPlayerInside;
        public bool isDogInside;
    }

    [Header("This system is convoluted and doesn't really work so well. Sorry!")]
    public List<AreaData> areas = new List<AreaData>();
    public AreaData innermostPlayerArea;
    public AreaData innermostDogArea;

    public bool drawGizmos = true;

    public delegate void AreaChangedDelegate(AreaData newArea, AreaData oldArea);

    public event AreaChangedDelegate OnPlayerAreaChanged;
    public event AreaChangedDelegate OnDogAreaChanged;

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
            if (innermostDogArea != null)
            {
                OnDogExitArea?.Invoke(innermostDogArea);
                innermostDogArea = null;
            }
            if (innermostPlayerArea != null)
            {
                OnPlayerExitArea?.Invoke(innermostPlayerArea);
                innermostPlayerArea = null;
            }
            return;
        }

        var playerPos = DogCastleReferences.instance.dogBrain.player.transform.position;
        var playerDist = Vector3.Distance(playerPos, transform.position);
        var dogPos = DogCastleReferences.instance.dogBrain.transform.position;
        if (debugDawg != null)
            dogPos = debugDawg.position;
        var dogDist = Vector3.Distance(dogPos, transform.position);
        var oldPlayerArea = innermostPlayerArea;
        var oldDogArea = innermostDogArea;

        // innermost area = the smallest radius that the player/dog is inside.
        innermostPlayerArea = null; // if too far from any area, it's null.
        innermostDogArea = null;

        // area large to small
        for (int i = areas.Count - 1; i >= 0; i--)
        {
            var area = areas[i];
            area.isPlayerInside = playerDist < area.areaRadius;
            area.isDogInside = dogDist < area.areaRadius;

            if (playerDist < area.areaRadius)
            {
                innermostPlayerArea = area;
            }
            if (dogDist < area.areaRadius)
            {
                innermostDogArea = area;
            }
        }

        // changed player area
        if (innermostPlayerArea != oldPlayerArea)
        {
            if (oldPlayerArea != null)
            {
                oldPlayerArea.isPlayerInside = false;
                OnPlayerExitArea?.Invoke(oldPlayerArea);
            }
            if (innermostPlayerArea != null)
            {
                innermostPlayerArea.isPlayerInside = true;
                OnPlayerEnterArea?.Invoke(innermostPlayerArea);
            }
            OnPlayerAreaChanged?.Invoke(innermostPlayerArea, oldPlayerArea);
        }

        // changed dog area
        if (innermostDogArea != oldDogArea)
        {
            if (oldDogArea != null)
            {
                oldDogArea.isDogInside = false;
                OnDogExitArea?.Invoke(oldDogArea);
            }
            if (innermostDogArea != null)
            {
                innermostDogArea.isDogInside = true;
                OnDogEnterArea?.Invoke(innermostDogArea);
            }
            OnDogAreaChanged?.Invoke(innermostDogArea, oldDogArea);
        }

    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;
            
        foreach (var area in areas)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, area.areaRadius);
        }
    }

}
