#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AudioLoom))]
public class SoundscapeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AudioLoom myScript = (AudioLoom)target;

        if (GUILayout.Button("Start"))
        {
            myScript.Activate(true);
            myScript.FadeTracksAdvanced();
        }

        if (GUILayout.Button("Stop"))
        {
            myScript.Activate(false);
            myScript.FullFadeOut(0.3f);
        }

        if (GUILayout.Button("Change Simple"))
        {
            myScript.ChangeSimple();
        }

        if (GUILayout.Button("Change Advanced"))
        {
            myScript.ChangeAdvanced();
        }

        if (GUILayout.Button("Current State Index +1"))
        {
            myScript.ChangeCurrentIndex(myScript.currentStateIndex + 1);
        }

        if (GUILayout.Button("Current State Index -1"))
        {
            myScript.ChangeCurrentIndex(myScript.currentStateIndex - 1);
        }

    }
    
}

#endif