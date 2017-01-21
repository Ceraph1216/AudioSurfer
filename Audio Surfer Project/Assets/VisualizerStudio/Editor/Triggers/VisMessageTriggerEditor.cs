using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(VisMessageTrigger))]
public class VisMessageTriggerEditor : VisTargetTriggerEditor 
{
    public VisMessageTriggerEditor()
    {
        showAutomaticInspectorGUI = false;
    }

    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisMessageTrigger trigger = target as VisMessageTrigger;
        if (trigger == null)
            return;

        trigger.messageName = EditorGUILayout.TextField("  Message Name", trigger.messageName);
        trigger.messageParameter = (VisMessageTrigger.ControllerSourceValue)EditorGUILayout.EnumPopup("  Message Parameter", (Enum)trigger.messageParameter);
    }
}