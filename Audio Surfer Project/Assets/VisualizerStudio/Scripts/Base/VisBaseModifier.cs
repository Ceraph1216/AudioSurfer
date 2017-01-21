using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class is a base class for all modifiers.  Modifiers are scripts that 
/// target a controller, and can modify game objects over time. 
/// </summary>
public abstract class VisBaseModifier : MonoBehaviour, IVisManagerTarget, IVisBaseControllerTarget
{		
	#region Defaults Static Class
	
	/// <summary>
    /// This internal class holds all of the defaults of the VisBaseModifier class. 
	/// </summary>
	public static class Defaults
	{					
	}
	
	#endregion
	
	#region IVisManagerTarget Implementation
	
	/// <summary>
	///	This is the vis manager that this modifier is targeting.
	/// </summary>
	//[HideInInspector()]
	[SerializeField()]
	private VisManager m_oVisManager = null;

    /// <summary>
    /// This is the name of the last manager that was set to this base modifier
    /// </summary>
    [HideInInspector()]
    [SerializeField()]
    private string m_szLastVisManagerName = null;
		
	/// <summary>
	/// This property gets/sets the target manager for this modifier. 
	/// </summary>
	public VisManager Manager
	{
		get { return m_oVisManager; }
		set 
		{ 			
			m_oVisManager = value;
            if (m_oVisManager != null)
                m_szLastVisManagerName = m_oVisManager.name;
            else
                m_szLastVisManagerName = null;
		}
    }

    /// <summary>
    /// This gets the name of the last manager that was set to this target.
    /// </summary>
    public string LastManagerName
    {
        get { return m_szLastVisManagerName; }
    }
	
	#endregion
	
	#region IVisBaseControllerTarget Implementation
	
	/// <summary>
	///	This is the controller that this modifier is targeting.
	/// </summary>
	//[HideInInspector()]
	[SerializeField()]
    private VisBaseController controller = null;

    /// <summary>
    /// This is the name of the last controller that was set to this base modifier
    /// </summary>
    [HideInInspector()]
    [SerializeField()]
    private string m_szLastControllerName = null;	
	
	/// <summary>
	/// This property gets/sets the target controller for this modifier. 
	/// </summary>
	public VisBaseController Controller
	{
		get { return controller; }
		set 
        {
            controller = value;
            if (controller != null)
                m_szLastControllerName = controller.controllerName;
            else
                m_szLastControllerName = null;
        }
	}

    /// <summary>
    /// This gets the name of the last controller that was set to this target.
    /// </summary>
    public string LastControllerName
    {
        get { return m_szLastControllerName; }
    }
	
	#endregion
	
	#region Init/Deinit Functions
	
	/// <summary>
	/// This function is called when this modifier is reset.
	/// Should be overriden by sub classes to reset variables 
	/// to their default values.
	/// </summary>
	public virtual void Reset()
	{	
	}	
	
	/// <summary>
	/// This function is called when this modifier is started.
	/// Should be override by sub classes to initialize.
	/// </summary>
	public virtual void Start () 
	{
        //make sure to restore the targets if needed
        VisManager.RestoreVisManagerTarget(this);
        VisBaseController.RestoreVisBaseControllerTarget(this);
	}
		
	/// <summary>
	/// This function is called when this modifier is destroyed.
	/// Should be override by sub classes to handle destruction.
	/// </summary>
	public virtual void OnDestroy()
	{
	}	
	
	#endregion
	
	#region Update Functions
	
	/// <summary>
	/// This updates this modifier.  This should not be overridden!  To implement custom modifier functionality, override OnValueUpdated().
	/// </summary>
	public void Update () 
	{	
		//make sure there is a controller and it is enabled, then call that the value was udpated
		if (Controller != null && Controller.enabled)
			OnValueUpdated(Controller.GetCurrentValue(),
		                   Controller.GetPreviousValue(),
		                   Controller.GetValueDifference(),
		                   Controller.GetAdjustedValueDifference());
	}
	
	/// <summary>
	/// This function is called by the base modifier whenever 
	/// the value of the targeted controller is updated.
	/// TO IMPLEMENT A CUSTOM MODIFIER, override this function 
	/// to handle the value being updated.
	/// </summary>
	/// <param name="current">
	/// The current value of the targeted controller.
	/// </param>
	/// <param name="previous">
	/// The previous value of the targeted controller.
	/// </param>
	/// <param name="difference">
	/// The value difference of the targeted controller.
	/// </param>
	/// <param name="adjustedDifference">
	/// The adjusted value difference of the targeted controller.
    /// This value is the difference value as if it took place over a 
    /// certain time period, controlled by VisBaseController.mc_fTargetAdjustedDifferenceTime.  The 
    /// default of this essientially indicates a frame rate of 60 fps to determine 
    /// the adjusted difference.  This should be used for almost all difference 
    /// calculations, as it is NOT frame rate dependent.
	/// </param>
	public abstract void OnValueUpdated(float current, float previous, float difference, float adjustedDifference);
	
	#endregion    
}

