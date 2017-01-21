using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

/// <summary>
/// This class is used as the custom editor for data groups.
/// </summary>
[CustomEditor(typeof(VisDataGroup))]
public sealed class VisDataGroupEditor : VisCommonEditor
{
    public VisDataGroupEditor()
    {
        showAutomaticInspectorGUI = false;
    }

    /// <summary>
    /// This is the function that is called by the base editor in order to display the custom inspector gui for required target objects.
    /// </summary>
    /// <returns>Whether or not the custom inspector gui found valid targets.</returns>
    protected override bool TargetInspectorGUI()
    {
        bool result = DisplayIVisManagerTargetGUI(target as IVisManagerTarget);

        VisDataGroup dataGroup = target as VisDataGroup;
        if (dataGroup != null &&
            !dataGroup.ValidateManager(false))
        {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.white;
            style.wordWrap = true;
            style.alignment = TextAnchor.MiddleCenter;

            Color oldColor = GUI.color;
            GUI.color = new Color(1.0f, 0.0f, 0.0f);
            GUILayout.Label("To prevent issues, please make sure this Data Group is attached to the same Game Object as it's Manager.", style);
            GUI.color = oldColor;
        }

        return result;
    }

    /// <summary>
    /// This function is called by the base editor to display normal custom inspector gui.
    /// </summary>
    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisDataGroup dataGroup = target as VisDataGroup;
        if (dataGroup == null)
            return;

        dataGroup.dataGroupName = EditorGUILayout.TextField("  Data Group Name", dataGroup.dataGroupName);
        dataGroup.numberSubDataGroups = Math.Abs(EditorGUILayout.IntField("  Sub Group Count", dataGroup.numberSubDataGroups));
        dataGroup.frequencyRangeStartIndex = EditorGUILayout.IntSlider("  Frequency Range Start Index", dataGroup.frequencyRangeStartIndex, 0, dataGroup.frequencyRangeEndIndex - 1);
        dataGroup.frequencyRangeEndIndex = EditorGUILayout.IntSlider("  Frequency Range End Index", dataGroup.frequencyRangeEndIndex, dataGroup.frequencyRangeStartIndex + 1, (dataGroup.Manager != null ? (int)dataGroup.Manager.windowSize : 4096) - 1);
        dataGroup.boost = Mathf.Abs(EditorGUILayout.FloatField("  Boost", dataGroup.boost));
        dataGroup.cutoff = Mathf.Abs(EditorGUILayout.FloatField("  Cutoff", dataGroup.cutoff));
        dataGroup.debugColor = EditorGUILayout.ColorField("  Debug Color", dataGroup.debugColor);
    }
}