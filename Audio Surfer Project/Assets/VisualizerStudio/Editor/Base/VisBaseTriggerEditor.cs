using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

/// <summary>
/// This class is used as the base editor for all custom trigger editors.
/// </summary>
[CustomEditor(typeof(VisBaseTrigger))]
public class VisBaseTriggerEditor : VisCommonEditor
{
    /// <summary>
    /// This is the function that is called by the base editor in order to display the custom inspector gui.
    /// </summary>
    /// <returns>Whether or not the custom inspector gui was finished.</returns>
    protected override bool TargetInspectorGUI()
	{
		return DisplayIVisManagerTargetGUI(target as IVisManagerTarget) && DisplayIVisBaseControllerTargetGUI(target as IVisBaseControllerTarget);
    }

    /// <summary>
    /// This function is called by the base editor to display normal custom inspector gui.
    /// </summary>
    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisBaseTrigger trigger = target as VisBaseTrigger;
        if (trigger == null)
            return;

        trigger.triggerType = (VisBaseTrigger.TriggerType)EditorGUILayout.EnumPopup("  Trigger Type", (Enum)trigger.triggerType);
        if (trigger.triggerType != VisBaseTrigger.TriggerType.None)
        {
            trigger.triggerThreshold = EditorGUILayout.FloatField("    Threshold", trigger.triggerThreshold);
            trigger.triggerReactivateDelay = EditorGUILayout.FloatField("    Reactivate Delay", trigger.triggerReactivateDelay);
            trigger.triggerRandomReactivateDelay = EditorGUILayout.FloatField("    Random Reactivate Delay", trigger.triggerRandomReactivateDelay);
        }
    }
}