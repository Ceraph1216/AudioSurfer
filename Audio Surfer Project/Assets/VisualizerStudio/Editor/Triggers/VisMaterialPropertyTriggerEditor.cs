using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VisMaterialPropertyTrigger))]
public class VisMaterialPropertyTriggerEditor : VisBasePropertyTriggerEditor 
{
    public VisMaterialPropertyTriggerEditor()
    {
        showAutomaticInspectorGUI = false;
    }

    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisMaterialPropertyTrigger trigger = target as VisMaterialPropertyTrigger;
        if (trigger == null)
            return;

        trigger.targetProperty = EditorGUILayout.TextField("  Target Property", trigger.targetProperty);
        trigger.setAsProceduralMaterial = EditorGUILayout.Toggle("  Set as Procedural Material", trigger.setAsProceduralMaterial);
        if (trigger.setAsProceduralMaterial)
            trigger.rebuildProceduralTexturesImmediately = EditorGUILayout.Toggle("    Rebuild Immediately", trigger.rebuildProceduralTexturesImmediately);
        trigger.applyToMaterialIndex = EditorGUILayout.Toggle("  Apply to Material Index", trigger.applyToMaterialIndex);
        if (trigger.applyToMaterialIndex)
            trigger.materialIndex = EditorGUILayout.IntField("    Index", trigger.materialIndex);
    }
}