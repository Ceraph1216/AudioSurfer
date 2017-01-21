using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

/// <summary>
/// This class is used as the base editor for all custom property modifier editors.
/// </summary>
[CustomEditor(typeof(VisBasePropertyTrigger))]
public class VisBasePropertyTriggerEditor : VisBaseTriggerEditor
{
    private enum PropertyValueType
    {
        NormalRange,
        InvertedRange,
        RandomRange,
    };

    /// <summary>
    /// This function is called by the base editor to display normal custom inspector gui.
    /// </summary>
    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisBasePropertyTrigger trigger = target as VisBasePropertyTrigger;
        if (trigger == null)
            return;

        trigger.controllerSourceValue = (ControllerSourceValue)EditorGUILayout.EnumPopup("  Controller Source Value", (Enum)trigger.controllerSourceValue);

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
        trigger.minPropertyValue = EditorGUILayout.FloatField("    Min Property Value", trigger.minPropertyValue);
        trigger.maxPropertyValue = EditorGUILayout.FloatField("    Max Property Value", trigger.maxPropertyValue);

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

        trigger.limitIncreaseRate = EditorGUILayout.Toggle("  Limit Increase Rate", trigger.limitIncreaseRate);
        if (trigger.limitIncreaseRate)
            trigger.increaseRate = EditorGUILayout.FloatField("    Increase Rate", trigger.increaseRate);
        trigger.limitDecreaseRate = EditorGUILayout.Toggle("  Limit Decrease Rate", trigger.limitDecreaseRate);
        if (trigger.limitDecreaseRate)
            trigger.decreaseRate = EditorGUILayout.FloatField("    Decrease Rate", trigger.decreaseRate);

        trigger.returnToRest = EditorGUILayout.Toggle("  Return to Rest", trigger.returnToRest);
        if (trigger.returnToRest)
            trigger.restValue = EditorGUILayout.FloatField("    Rest Value", trigger.restValue);
    }
}