using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VisSpawnPrefabTrigger))]
public class VisSpawnPrefabTriggerEditor : VisBaseTriggerEditor 
{
    public VisSpawnPrefabTriggerEditor()
    {
        showAutomaticInspectorGUI = false;
    }

    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisSpawnPrefabTrigger trigger = target as VisSpawnPrefabTrigger;
        if (trigger == null)
            return;

        trigger.prefab = (GameObject)EditorGUILayout.ObjectField("  Prefab", trigger.prefab, typeof(GameObject), false);

#if (UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
        EditorGUIUtility.LookLikeControls();
#endif

        trigger.randomOffset = EditorGUILayout.Vector3Field("  Random Offset", trigger.randomOffset);

#if (UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
        EditorGUIUtility.LookLikeInspector();
#endif
    }
}