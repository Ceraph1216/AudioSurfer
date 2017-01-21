using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(VisMicrophone))]
public class VisMicrophoneEditor : Editor 
{
    public VisMicrophoneEditor()
    {
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;

        base.OnInspectorGUI();

        List<string> inputs = new List<string>();
        inputs.Add("[Select a Preset]");
        inputs.Add("Default");

        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            inputs.Add(Microphone.devices[i]);
        }

        EditorGUILayout.BeginHorizontal();

        int index = 0;
        index = EditorGUILayout.Popup("            Preset", index, inputs.ToArray());
        if (index > 0)
            (target as VisMicrophone).microphoneDevice = inputs[index];

        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
       
#if UNITY_5 
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
        {
            EditorGUILayout.HelpBox("VisMicrophone is not supported for WebGL", MessageType.Warning);
        }
#endif
    }
}