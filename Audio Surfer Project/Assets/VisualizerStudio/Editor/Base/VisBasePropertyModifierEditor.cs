using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

/// <summary>
/// This class is used as the base editor for all custom property modifier editors.
/// </summary>
[CustomEditor(typeof(VisBasePropertyModifier))]
public class VisBasePropertyModifierEditor : VisBaseModifierEditor
{
    private enum PropertyValueType
    {
        NormalRange,
        InvertedRange
    };

    /// <summary>
    /// This function is called by the base editor to display normal custom inspector gui.
    /// </summary> 
    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisBasePropertyModifier modifier = target as VisBasePropertyModifier;
        if (modifier == null)
            return;

        modifier.controllerSourceValue = (ControllerSourceValue)EditorGUILayout.EnumPopup("  Controller Source Value", (Enum)modifier.controllerSourceValue);

        PropertyValueType rules = PropertyValueType.NormalRange;
        if (modifier.invertValue)
            rules = PropertyValueType.InvertedRange;

        rules = (PropertyValueType)EditorGUILayout.EnumPopup("  Property Value Type", (Enum)rules);
        if (rules == PropertyValueType.NormalRange || rules == PropertyValueType.InvertedRange)
        {
            modifier.minControllerValue = EditorGUILayout.FloatField("    Min Controller Value", modifier.minControllerValue);
            modifier.maxControllerValue = EditorGUILayout.FloatField("    Max Controller Value", modifier.maxControllerValue);
        }
        modifier.minPropertyValue = EditorGUILayout.FloatField("    Min Property Value", modifier.minPropertyValue);
        modifier.maxPropertyValue = EditorGUILayout.FloatField("    Max Property Value", modifier.maxPropertyValue);

        if (rules == PropertyValueType.NormalRange)
        {
            modifier.invertValue = false;
        }
        else if (rules == PropertyValueType.InvertedRange)
        {
            modifier.invertValue = true;
        }
    }
} 