using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// This class is used as the base editor for all custom modifier editors.
/// </summary>
[CustomEditor(typeof(VisBaseModifier))]
public class VisBaseModifierEditor : VisCommonEditor
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
    }
} 