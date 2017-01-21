using UnityEngine;
using UnityEditor;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

/// <summary>
/// This is the editor for the VisManager.
/// </summary>
[CustomEditor(typeof(VisManager))]
public class VisManagerEditor : UnityEditor.Editor
{
    private bool displayDebugFoldOut = false;

    /// <summary>
    /// Main inspector gui function.
    /// </summary>
    public override void OnInspectorGUI()
    {
        //make this look like inspect and make sure the target is valid
#if (UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
        EditorGUIUtility.LookLikeInspector();
#endif

        GUI.changed = false;

        VisManager manager = target as VisManager;
        if (manager == null)
            return;

        manager.audioSource = (AudioSource)EditorGUILayout.ObjectField("  Audio Source", manager.audioSource, typeof(AudioSource), true);
        manager.channel = (VisManager.Channel)EditorGUILayout.EnumPopup("  Audio Channel", (Enum)manager.channel);
        manager.windowSize = (VisManager.WindowSize)EditorGUILayout.EnumPopup("  Window Size", (Enum)manager.windowSize);
        manager.windowType = (UnityEngine.FFTWindow)EditorGUILayout.EnumPopup("  Window Type", (Enum)manager.windowType);

        displayDebugFoldOut = EditorGUILayout.Foldout(displayDebugFoldOut, "Debug");
        if (displayDebugFoldOut)
        {
            manager.displaySpectrumDebug = EditorGUILayout.Toggle("    Display Sectrum", manager.displaySpectrumDebug);
            if (manager.displaySpectrumDebug)
            {
                manager.debugSpectrumBarWidth = Math.Abs(EditorGUILayout.IntField("      Spectrum Bar Width", manager.debugSpectrumBarWidth));
                manager.debugSpectrumBarHeight = Math.Abs(EditorGUILayout.IntField("      Spectrum Bar Height", manager.debugSpectrumBarHeight));
                manager.debugRawAudioBarHeight = Math.Abs(EditorGUILayout.IntField("      Raw Audio Bar Height", manager.debugRawAudioBarHeight));
            }

            manager.displayDataGroupDebug = EditorGUILayout.Toggle("    Display Data Group", manager.displayDataGroupDebug);
            if (manager.displayDataGroupDebug)
            {
                manager.debugDataGroupBarWidth = Math.Abs(EditorGUILayout.IntField("      Bar Width", manager.debugDataGroupBarWidth));
                manager.debugDataGroupBarHeight = Math.Abs(EditorGUILayout.IntField("      Bar Height", manager.debugDataGroupBarHeight));
            }

            manager.displayControllerDebug = EditorGUILayout.Toggle("    Display Controller", manager.displayControllerDebug);
            if (manager.displayControllerDebug)
            {
                manager.debugControllerBarWidth = Math.Abs(EditorGUILayout.IntField("      Bar Width", manager.debugControllerBarWidth));
                manager.debugControllerBarHeight = Math.Abs(EditorGUILayout.IntField("      Bar Height", manager.debugControllerBarHeight));
            }

            manager.debugSeparation = Math.Abs(EditorGUILayout.IntField("    Separation", manager.debugSeparation));
            manager.debugTexture = (Texture)EditorGUILayout.ObjectField("    Texture", manager.debugTexture, typeof(Texture), false);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
