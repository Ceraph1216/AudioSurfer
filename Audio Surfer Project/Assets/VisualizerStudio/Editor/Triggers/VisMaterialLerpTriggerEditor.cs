using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VisMaterialLerpTrigger))]
public class VisMaterialLerpTriggerEditor : VisBasePropertyTriggerEditor 
{
    public VisMaterialLerpTriggerEditor()
    {
        showAutomaticInspectorGUI = false;
    }

    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisMaterialLerpTrigger trigger = target as VisMaterialLerpTrigger;
        if (trigger == null)
            return;

        trigger.lerpFromMaterial = (Material)EditorGUILayout.ObjectField("  Lerp from Material", trigger.lerpFromMaterial, typeof(Material), false);
        trigger.lerpToMaterial = (Material)EditorGUILayout.ObjectField("  Lerp to Material", trigger.lerpToMaterial, typeof(Material), false);
    }
}