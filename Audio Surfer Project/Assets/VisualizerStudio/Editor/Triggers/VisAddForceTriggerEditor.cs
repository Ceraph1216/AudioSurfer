using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(VisAddForceTrigger))]
public class VisAddForceTriggerEditor : VisBaseTriggerEditor
{
    private enum PropertyValueType
    {
        NormalRange,
        InvertedRange,
        RandomRange,
    };

    public VisAddForceTriggerEditor()
    {
        showAutomaticInspectorGUI = false;
    }

    /// <summary>
    /// This function is called by the base editor to display normal custom inspector gui.
    /// </summary>
    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisAddForceTrigger trigger = target as VisAddForceTrigger;
        if (trigger == null)
            return;

        trigger.controllerValue = (ControllerSourceValue)EditorGUILayout.EnumPopup("  Controller Source Value", (Enum)trigger.controllerValue);

        PropertyValueType rules = PropertyValueType.NormalRange;
        if (trigger.randomValue)
            rules = PropertyValueType.RandomRange;
        else if (trigger.invertValue)
            rules = PropertyValueType.InvertedRange;

        rules = (PropertyValueType)EditorGUILayout.EnumPopup("  Property Value Type", (Enum)rules);
        if (rules == PropertyValueType.NormalRange || rules == PropertyValueType.InvertedRange)
        {
            trigger.minControllerValue = EditorGUILayout.FloatField("    Min Controller Value", trigger.minControllerValue);
            trigger.maxControllerValue = EditorGUILayout.FloatField("    Max Controller Value", trigger.maxControllerValue);
        }
        trigger.minForceValue = EditorGUILayout.FloatField("    Min Force Value", trigger.minForceValue);
        trigger.maxForceValue = EditorGUILayout.FloatField("    Max Force Value", trigger.maxForceValue);

        if (rules == PropertyValueType.NormalRange)
        {
            trigger.invertValue = false;
            trigger.randomValue = false;
        }
        else if (rules == PropertyValueType.InvertedRange)
        {
            trigger.invertValue = true;
            trigger.randomValue = false;
        }
        else if (rules == PropertyValueType.RandomRange)
        {
            trigger.invertValue = false;
            trigger.randomValue = true;
        }

        trigger.forceMode = (ForceMode)EditorGUILayout.EnumPopup("  Force Mode", (Enum)trigger.forceMode);

#if (UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
        EditorGUIUtility.LookLikeControls();
#endif

        trigger.forceDirection = EditorGUILayout.Vector3Field("  Force Direction", trigger.forceDirection);

#if (UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
        EditorGUIUtility.LookLikeInspector();
#endif
    }
}