using UnityEngine;
using UnityEditor;
using System.Collections.ObjectModel;
using System.Collections.Generic;

/// <summary>
/// This is the common base custom editor for visualizer studio objects and classes.
/// </summary>
public class VisCommonEditor : UnityEditor.Editor
{
    /// <summary>
    /// This indicates if the automatic inspector gui should be shown.
    /// </summary>
    public bool showAutomaticInspectorGUI = true;

	internal void EnsureAllDataGroupsRegistered()
	{
        if (target is MonoBehaviour)
        {
            VisManager manager = (target as MonoBehaviour).gameObject.GetComponent<VisManager>();
            VisDataGroup[] dataGroupArray = (target as MonoBehaviour).gameObject.GetComponents<VisDataGroup>();

            if (manager != null)
                manager.ClearDataGroups();
            for (int i = 0; i < dataGroupArray.Length; i++)
            {
                dataGroupArray[i].EnsureRegistered();
            }
        }

		Object[] visManagers = GameObject.FindObjectsOfType(typeof(VisManager));
		for (int i = 0; i < visManagers.Length; i++)
		{
			(visManagers[i] as VisManager).ClearDataGroups();
		}
			
		Object[] dataGroups = GameObject.FindObjectsOfType(typeof(VisDataGroup));
		for (int i = 0; i < dataGroups.Length; i++)
		{
			if (dataGroups[i])
				(dataGroups[i] as VisDataGroup).EnsureRegistered();
        }
	}
	
	internal void EnsureAllControllersRegistered()
	{
        if (target is MonoBehaviour)
        {
            VisManager manager = (target as MonoBehaviour).gameObject.GetComponent<VisManager>();
            VisBaseController[] controllerArray = (target as MonoBehaviour).gameObject.GetComponents<VisBaseController>();

            if (manager != null)
                manager.ClearDataGroups();
            for (int i = 0; i < controllerArray.Length; i++)
            {
                controllerArray[i].EnsureRegistered();
            }
        }

		Object[] visManagers = GameObject.FindObjectsOfType(typeof(VisManager));
		for (int i = 0; i < visManagers.Length; i++)
		{
			(visManagers[i] as VisManager).ClearControllers();
		}
		
		Object[] controllers = GameObject.FindObjectsOfType(typeof(VisBaseController));
		for (int i = 0; i < controllers.Length; i++)
		{
			if (controllers[i])
				(controllers[i] as VisBaseController).EnsureRegistered();
        }
	}
    
    /// <summary>
    /// This displays the drop down for selecting a manager from the inspector.
    /// </summary>
    /// <param name="managerTarget">The manager target to set the manager for.</param>
    /// <returns>Whether or not a manager is currently set.</returns>
	public bool DisplayIVisManagerTargetGUI(IVisManagerTarget managerTarget)
	{
		if (managerTarget != null)
		{
            if (managerTarget is MonoBehaviour)
            {
                GameObject gameObject = (managerTarget as MonoBehaviour).gameObject;
                if (managerTarget.Manager == null ||
                    managerTarget.Manager.gameObject != gameObject)
                {
                    VisManager manager = gameObject.GetComponent<VisManager>();
                    if (manager != null)
                    {
                        managerTarget.Manager = manager;
                        return true;
                    }
                }
                else if (managerTarget.Manager != null &&
                         managerTarget.Manager.gameObject == gameObject)
                {
                    return true;
                }
            }

            bool result = false;

            EditorGUILayout.BeginHorizontal();

            //make sure and try to restore it first
            VisManager.RestoreVisManagerTarget(managerTarget);

			Object[] visManagers = GameObject.FindObjectsOfType(typeof(VisManager));
			
			if (visManagers.Length > 0)
			{
				//create list of vis manager names and a dictionary to map IDs, and sort it
				List<string> sortedNames = new List<string>(visManagers.Length);
				Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>(visManagers.Length);
				for (int i = 0; i < visManagers.Length; i++)
				{
                    string name = (visManagers[i] as VisManager).name;
                    if (nameToIndexMap.ContainsKey(name) &&
                        ((visManagers[nameToIndexMap[name]] as VisManager).Controllers.Count < (visManagers[i] as VisManager).Controllers.Count ||
                         (visManagers[nameToIndexMap[name]] as VisManager).DataGroups.Count < (visManagers[i] as VisManager).DataGroups.Count))
                    {//override index
                        nameToIndexMap[name] = i;
                    }
                    else
                    {//add new
                        sortedNames.Add((visManagers[i] as VisManager).name);
                        nameToIndexMap.Add((visManagers[i] as VisManager).name, i);
                    }
				}
				sortedNames.Sort();
				
				//create array of names and set current index
				int currentIndex = 0;
				string[] displayedNames = new string[visManagers.Length + 1];
				displayedNames[0] = "None";
				for (int i = 0; i < sortedNames.Count; i++)
				{
					displayedNames[i + 1] = sortedNames[i];
					if (managerTarget.Manager == visManagers[nameToIndexMap[sortedNames[i]]])
						currentIndex = i + 1;
				}
				
				//display popup
				int newIndex = EditorGUILayout.Popup("   Manager", currentIndex, displayedNames);
				
				//set new vis manager if the index has changed
				if (newIndex != currentIndex)
				{
					if (newIndex == 0)
						managerTarget.Manager = null;
					else
					{
						string newName = sortedNames[newIndex - 1];
						int remappedIndex = nameToIndexMap[newName];
						managerTarget.Manager = visManagers[remappedIndex] as VisManager;
					}
					EditorUtility.SetDirty(target);
                }
				result =  managerTarget.Manager != null;
			}
			else
            {
                if (managerTarget.LastManagerName != null && managerTarget.LastManagerName.Length > 0)
                    EditorGUILayout.LabelField("   Manager", managerTarget.LastManagerName + " (not found, try selecting the Object with this Manager)");
                else
                    EditorGUILayout.LabelField("   Manager", "NO MANAGERS FOUND!");
				result = false;
			}

            managerTarget.Manager = (VisManager)EditorGUILayout.ObjectField(managerTarget.Manager, typeof(VisManager), true, GUILayout.Width(Screen.width * 0.2f));

            EditorGUILayout.EndHorizontal();

            return result;
		}
		return false;
	}
    
    /// <summary>
    /// This displays the drop down for selecting a data group from the inspector.
    /// </summary>
    /// <param name="dataGroupTarget">The data group target to set the data group for.</param>
    /// <returns>Whether or not a data group is currently set.</returns>
	public bool DisplayIVisDataGroupTargetGUI(IVisDataGroupTarget dataGroupTarget)
	{
		EnsureAllDataGroupsRegistered();
		
		if (dataGroupTarget != null && dataGroupTarget is IVisManagerTarget)
        {
            //make sure and try to restore it first
            VisDataGroup.RestoreVisDataGroupTarget(dataGroupTarget);

			VisManager manager = (dataGroupTarget as IVisManagerTarget).Manager;
			if (manager != null)
			{	
				ReadOnlyCollection<VisDataGroup> dataGroups = manager.DataGroups;
				if (dataGroups.Count > 0)
				{					
					//create list of vis data group names and a dictionary to map IDs, and sort it
					List<string> sortedNames = new List<string>(dataGroups.Count);
					Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>(dataGroups.Count);
					for (int i = 0; i < dataGroups.Count; i++)
					{
						sortedNames.Add((dataGroups[i] as VisDataGroup).dataGroupName);
						nameToIndexMap.Add((dataGroups[i] as VisDataGroup).dataGroupName, i);
					}
					sortedNames.Sort();
					
					//create array of names and set current index
					int currentIndex = 0;
					string[] displayedNames = new string[dataGroups.Count + 1];
					displayedNames[0] = "None";
					for (int i = 0; i < sortedNames.Count; i++)
					{
						displayedNames[i + 1] = sortedNames[i];
						if (dataGroupTarget.DataGroup == dataGroups[nameToIndexMap[sortedNames[i]]])
							currentIndex = i + 1;
					}
					
					//display popup
					int newIndex = EditorGUILayout.Popup("   Data Group", currentIndex, displayedNames);
					
					//set new vis data group if the index has changed
					if (newIndex != currentIndex)
					{
						if (newIndex == 0)
							dataGroupTarget.DataGroup = null;
						else
						{
							string newName = sortedNames[newIndex - 1];
							int remappedIndex = nameToIndexMap[newName];
							dataGroupTarget.DataGroup = dataGroups[remappedIndex] as VisDataGroup;
						}
						EditorUtility.SetDirty(target);
					}
					return dataGroupTarget.DataGroup != null;
				}
				else
				{
                    if (dataGroupTarget.LastDataGroupName != null && dataGroupTarget.LastDataGroupName.Length > 0)
                        EditorGUILayout.LabelField("   Data Group", dataGroupTarget.LastDataGroupName + " (not found, try selecting the Object with this Data Group)");
                    else
    					EditorGUILayout.LabelField("   Data Group", "NO DATA GROUPS FOUND!");
					return false;
				}
			}
		}
		return false;
	}
	
    /// <summary>
    /// This displays the drop down for selecting a controller from the inspector.
    /// </summary>
    /// <param name="baseControllerTarget">The base controller target to set the controller for.</param>
    /// <returns>Whether or not a controller is currently set.</returns>
    public bool DisplayIVisBaseControllerTargetGUI(IVisBaseControllerTarget baseControllerTarget)
	{
		EnsureAllControllersRegistered();
		
		if (baseControllerTarget != null && baseControllerTarget is IVisManagerTarget)
        {
            //make sure and try to restore it first
            VisBaseController.RestoreVisBaseControllerTarget(baseControllerTarget);

			VisManager manager = (baseControllerTarget as IVisManagerTarget).Manager;
			if (manager != null)
			{	
				ReadOnlyCollection<VisBaseController> controllers = manager.Controllers;
				if (controllers.Count > 0)
				{					
					//create list of vis controller names and a dictionary to map IDs, and sort it
					List<string> sortedNames = new List<string>(controllers.Count);
					Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>(controllers.Count);
					for (int i = 0; i < controllers.Count; i++)
					{
						sortedNames.Add((controllers[i] as VisBaseController).controllerName);
						nameToIndexMap.Add((controllers[i] as VisBaseController).controllerName, i);
					}
					sortedNames.Sort();
					
					//create array of names and set current index
					int currentIndex = 0;
					string[] displayedNames = new string[controllers.Count + 1];
					displayedNames[0] = "None";
					for (int i = 0; i < sortedNames.Count; i++)
					{
						displayedNames[i + 1] = sortedNames[i];
						if (baseControllerTarget.Controller == controllers[nameToIndexMap[sortedNames[i]]])
							currentIndex = i + 1;
					}
					
					//display popup
					int newIndex = EditorGUILayout.Popup("   Controller", currentIndex, displayedNames);
					
					//set new vis controller if the index has changed
					if (newIndex != currentIndex)
					{
						if (newIndex == 0)
							baseControllerTarget.Controller = null;
						else
						{
							string newName = sortedNames[newIndex - 1];
							int remappedIndex = nameToIndexMap[newName];
							baseControllerTarget.Controller = controllers[remappedIndex] as VisBaseController;
						}
						EditorUtility.SetDirty(target);
					}
					return baseControllerTarget.Controller != null;
				}
				else
                {
                    if (baseControllerTarget.LastControllerName != null && baseControllerTarget.LastControllerName.Length > 0)
                        EditorGUILayout.LabelField("   Controller", baseControllerTarget.LastControllerName + " (not found, try selecting the Object with this Controller)");
					else
                        EditorGUILayout.LabelField("   Controller", "NO CONTROLLERS FOUND!");
					return false;
				}
			}
		}
		return false;
	}

    /// <summary>
    /// This is called when the inspector gui needs to be displayed.
    /// </summary>
    public override void OnInspectorGUI()
    {
        GUI.changed = false;

        if (TargetInspectorGUI())
        {
            EditorGUILayout.Separator();
            CustomInspectorGUI();
            if (showAutomaticInspectorGUI)
                DisplayAutomaticInspectorGUI();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    protected virtual bool TargetInspectorGUI()
    {
        return true;
    }

    /// <summary>
    /// This function is called by the base editor to display normal custom inspector gui.
    /// </summary>
    protected virtual void CustomInspectorGUI()
    {
#if (UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
        EditorGUIUtility.LookLikeInspector();
#endif
    }

    /// <summary>
    /// This displays the automatic inspector gui.  Call this if you have public members
    /// you wish to edit automatically without manually setting up your inspector gui.
    /// </summary>
    private void DisplayAutomaticInspectorGUI()
    {
        EditorGUILayout.Separator();
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        EditorGUILayout.Foldout(true, "Automatic " + target.GetType().ToString() + " Properties", style);
        base.OnInspectorGUI();
    }
}

