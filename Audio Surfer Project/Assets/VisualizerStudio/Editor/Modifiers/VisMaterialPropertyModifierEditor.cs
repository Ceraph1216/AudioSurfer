using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VisMaterialPropertyModifier))]
public class VisMaterialPropertyModifierEditor : VisBasePropertyModifierEditor 
{
    public VisMaterialPropertyModifierEditor()
    {
        showAutomaticInspectorGUI = false;
    }

    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisMaterialPropertyModifier modifier = target as VisMaterialPropertyModifier;
        if (modifier == null)
            return;

        modifier.targetProperty = EditorGUILayout.TextField("  Target Property", modifier.targetProperty);
        modifier.setAsProceduralMaterial = EditorGUILayout.Toggle("  Set as Procedural Material", modifier.setAsProceduralMaterial);
        if (modifier.setAsProceduralMaterial)
            modifier.rebuildProceduralTexturesImmediately = EditorGUILayout.Toggle("    Rebuild Immediately", modifier.rebuildProceduralTexturesImmediately);
        modifier.applyToMaterialIndex = EditorGUILayout.Toggle("  Apply to Material Index", modifier.applyToMaterialIndex);
        if (modifier.applyToMaterialIndex)
            modifier.materialIndex = EditorGUILayout.IntField("    Index", modifier.materialIndex);
    }
}