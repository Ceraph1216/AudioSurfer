﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VisMaterialLerpModifier))]
public class VisMaterialLerpModifierEditor : VisBasePropertyModifierEditor 
{
    public VisMaterialLerpModifierEditor()
    {
        showAutomaticInspectorGUI = false;
    }

    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisMaterialLerpModifier modifier = target as VisMaterialLerpModifier;
        if (modifier == null)
            return;

        modifier.lerpFromMaterial = (Material)EditorGUILayout.ObjectField("  Lerp from Material", modifier.lerpFromMaterial, typeof(Material), false);
        modifier.lerpToMaterial = (Material)EditorGUILayout.ObjectField("  Lerp to Material", modifier.lerpToMaterial, typeof(Material), false);
    }
}