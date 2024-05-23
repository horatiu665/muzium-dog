using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class WackyClock : MonoBehaviour
{
    public List<Transform> hours = new();
    public List<Transform> minutes = new();

    public float updateDelayInSeconds = 1f;
    private float nextUpdate = 0f;

    public void FindChildrenRecursive(Transform parent)
    {
        if (parent.name.Contains("hour"))
        {
            hours.Add(parent);
        }
        else if (parent.name.Contains("minute"))
        {
            minutes.Add(parent);
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            FindChildrenRecursive(parent.GetChild(i));
        }
    }

    [DebugButton]
    public void Editor_FindChildren()
    {
        hours.Clear();
        minutes.Clear();
        FindChildrenRecursive(transform);
    }

    void Update()
    {
        // update once every X seconds
        if (Time.time > nextUpdate)
        {
            nextUpdate = Time.time + updateDelayInSeconds;
            UpdateClock();
        }

    }

    private void UpdateClock()
    {
        var time = System.DateTime.Now;
        var hour = time.Hour;
        var minute = time.Minute;
        var second = time.Second;

        // update all hands
        for (int i = 0; i < hours.Count; i++)
        {
            // 360 degrees / 12 hours = 30 degrees per hour
            // 30 degrees per hour / 60 minutes = 0.5 degrees per minute
            hours[i].transform.localEulerAngles = new Vector3(0, 0, hour * -30 + minute * -0.5f);
        }

        for (int i = 0; i < minutes.Count; i++)
        {
            minutes[i].transform.localEulerAngles = new Vector3(0, 0, minute * -6 + second * -0.1f);
        }
    }
}
