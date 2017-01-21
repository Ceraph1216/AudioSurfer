using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// This class is used as the base editor for all custom controller editors.
/// </summary>
[CustomEditor(typeof(VisBaseController))]
public class VisBaseControllerEditor : VisCommonEditor
{	
    /// <summary>
    /// This is the function that is called by the base editor in order to display the custom inspector gui for required target objects.
    /// </summary>
    /// <returns>Whether or not the custom inspector gui found valid targets.</returns>
	protected override bool TargetInspectorGUI()
	{
		bool result = DisplayIVisManagerTargetGUI(target as IVisManagerTarget);

        VisBaseController controller = target as VisBaseController;
        if (controller != null &&
            !controller.ValidateManager(false))
        {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.white;
            style.wordWrap = true;
            style.alignment = TextAnchor.MiddleCenter;

            Color oldColor = GUI.color;
            GUI.color = new Color(1.0f, 0.0f, 0.0f);
            GUILayout.Label("To prevent issues, please make sure this Controller is attached to the same Game Object as it's Manager.", style);
            GUI.color = oldColor;
        }

        return result;
	}

    /// <summary>
    /// This function is called by the base editor to display normal custom inspector gui.
    /// </summary>
    protected override void CustomInspectorGUI()
    {
        base.CustomInspectorGUI();

        VisBaseController controller = target as VisBaseController;
        if (controller == null)
            return;

        controller.controllerName = EditorGUILayout.TextField("  Controller Name", controller.controllerName);
        controller.limitIncreaseRate = EditorGUILayout.Toggle("  Limit Increase Rate", controller.limitIncreaseRate);
        if (controller.limitIncreaseRate)
            controller.increaseRate = EditorGUILayout.FloatField("    Increase Rate", controller.increaseRate);
        controller.limitDecreaseRate = EditorGUILayout.Toggle("  Limit Decrease Rate", controller.limitDecreaseRate);
        if (controller.limitDecreaseRate)
            controller.decreaseRate = EditorGUILayout.FloatField("    Decrease Rate", controller.decreaseRate);
    }
}