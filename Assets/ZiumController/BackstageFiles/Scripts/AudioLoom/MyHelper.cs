using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MyHelper
{
    public static Vector3 LimitVector(Vector3 v, float maxMagnitude)
    {
        if (v.magnitude <= maxMagnitude)
            return v;

        return v.normalized * maxMagnitude;
    }

    public static float Modulo(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    public static float Map(float val, float fromMin, float fromMax, float toMin, float toMax)
    {
        return ((val - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin;
    }

  

   
    public static float GetSign(float f)
    {
        if (f >= 0f)
            return 1f;
        return -1f;
    }

    public static Vector2 Vector3to2(Vector3 _base, int indexToRemove)
    {
        if (indexToRemove == 0)
            return new Vector2(_base.y, _base.z);
        if (indexToRemove == 1)
            return new Vector2(_base.x, _base.z);
        return new Vector2(_base.x, _base.y);
    }

    public static Vector3 ReplaceIndex(Vector3 _base, int indexToReplace, float newValue)
    {
        Vector3 final = _base;
        final[indexToReplace] = newValue;
        return final;
    }

    public static Component SearchForComponentWithName<T>(GameObject parent, string nom)
    {

        if (parent.GetComponent(typeof(T)) != null)
            if (parent.gameObject.name == nom)
                return parent.GetComponent(typeof(T));


        if (parent.GetComponentsInChildren(typeof(T)).ToList().Count == 0)
            return null;

        if (parent.GetComponentsInChildren(typeof(T)).Where(a => a.gameObject.name == nom).ToList().Count == 0)
            return null;

        return parent.GetComponentsInChildren(typeof(T)).Where(a => a.gameObject.name == nom).ToList()[0];
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static List<int> GetListIndexes(int amount,bool shuffled = false)
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < amount; i++)
            indexes.Add(i);
        if(shuffled)
            MyHelper.Shuffle(indexes);

        return indexes;
    }



}


