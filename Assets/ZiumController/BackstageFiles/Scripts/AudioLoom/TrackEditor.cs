#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Track))]
public class TrackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Track myScript = (Track)target;
/*
        if (GUILayout.Button("Fade In"))
        {
            myScript.FadeIn();
        }

        if (GUILayout.Button("Fade In [FORCE]"))
        {
            myScript.FadeIn(true);
        }

        if (GUILayout.Button("Fade Out"))
        {
            myScript.FadeOut();
        }

        if (GUILayout.Button("Fade Out [FORCE]"))
        {
            myScript.FadeOut(true);
        }

        if (GUILayout.Button("Fade to Random"))
        {
            myScript.FromCurrentValueToRandomValue();
        }

        if (GUILayout.Button("Fade to Random [FORCE]"))
        {
            myScript.FromCurrentValueToRandomValue(true);
        }

        if (GUILayout.Button("Instant Play"))
        {
            myScript.InstantPlay();
        }

        if (GUILayout.Button("Instant Stop"))
        {
            myScript.InstantStop();
        }
        */

    }


}
#endif