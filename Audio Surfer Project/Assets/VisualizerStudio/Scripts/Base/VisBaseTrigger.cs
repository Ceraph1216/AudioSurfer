using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// This class is a base class for all triggers.  Triggers are scripts that 
/// target a controller, and can trigger events in the game, based on input 
/// from its target controller. 
/// </summary>
public abstract class VisBaseTrigger : MonoBehaviour, IVisManagerTarget, IVisBaseControllerTarget
{
	#region Defaults Static Class
	
	/// <summary>
	/// This internal class holds all of the defaults of the VisBaseTrigger class. 
	/// </summary>
	public static class Defaults
	{			
		public const TriggerType triggerType = TriggerType.GreaterThanChangeThreshold;
		public const float triggerThreshold = 0.1f;
		public const float triggerReactivateDelay = 0.25f;
        public const float triggerRandomReactivateDelay = 0.0f;
	}
	
	#endregion
	
	#region Enumerations
	
	/// <summary>
	/// This enumeration indicates how this trigger is activated.
	/// </summary>
	public enum TriggerType
	{
		/// <summary>
		/// This indicates no default triggering and it must be triggered by custom functionality. 
		/// </summary>
		None,
		
		/// <summary>
		/// This indicates that this trigger is triggered when the associated controllers value goes under the specified threshold. 
		/// </summary>
		LessThanValueThreshold,
		
		/// <summary>
		/// This indicates that this trigger is triggered when the associated controllers value goes over the specified threshold. 
		/// </summary>
		GreaterThanValueThreshold,
		
		/// <summary>
		/// This indicates that this trigger is triggered when the associated controllers value difference/change goes under the specified threshold. 
		/// </summary>
		LessThanChangeThreshold,
		
		/// <summary>
		/// This indicates that this trigger is triggered when the associated controllers value difference/change goes over the specified threshold. 
		/// </summary>
		GreaterThanChangeThreshold		
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
	
	#region Public Member Variables
	
    /// <summary>
    /// This describes the type of this trigger.  (i.e. How it functions)
    /// </summary>
    //[HideInInspector()]
	public TriggerType triggerType = Defaults.triggerType;

    /// <summary>
    /// This is the threshold at which the trigger should activate, based on the current Trigger Type.
    /// </summary>
    //[HideInInspector()]
	public float triggerThreshold = Defaults.triggerThreshold;

    /// <summary>
    /// This is the amount of time to wait in between consecutive triggers of this trigger.
    /// </summary>
    //[HideInInspector()]
	public float triggerReactivateDelay = Defaults.triggerReactivateDelay;

    /// <summary>
    /// This is the amount of random delay to add to the reactivate time.  
    /// Random range from "-value" to "value"
    /// </summary>
    //[HideInInspector()]
    public float triggerRandomReactivateDelay = Defaults.triggerRandomReactivateDelay;
	
	#endregion
	
	#region Protected Member Variables
	
    /// <summary>
    /// This is the timer used to control the trigger reactivate delay.
    /// </summary>
	protected float m_fTriggerDelayTimer = 0.0f;
	
	#endregion
		
	#region Init/Deinit Functions
	
	/// <summary>
	/// This function is called when this trigger is reset.
	/// Should be overriden by sub classes to reset variables 
	/// to their default values.
	/// </summary>
	public virtual void Reset()
	{	
		triggerType = Defaults.triggerType;
		triggerThreshold = Defaults.triggerThreshold;
		triggerReactivateDelay = Defaults.triggerReactivateDelay;	
	}	
	
	/// <summary>
	/// This function is called when this trigger is started.
	/// Should be override by sub classes to initialize.
	/// </summary>
	public virtual void Start ()
    {
        //make sure to restore the targets if needed
        VisManager.RestoreVisManagerTarget(this);
        VisBaseController.RestoreVisBaseControllerTarget(this);

		//validate trigger variables.
		triggerThreshold = VisHelper.Validate(triggerThreshold, 0.0001f, 10000.0f, Defaults.triggerThreshold, this, "triggerThreshold", false);
		triggerReactivateDelay = VisHelper.Validate(triggerReactivateDelay, 0.0f, 10000.0f, Defaults.triggerReactivateDelay, this, "triggerReactivateDelay", false);
	}
		
	/// <summary>
	/// This function is called when this trigger is destroyed.
	/// Should be override by sub classes to handle destruction.
	/// </summary>
	public virtual void OnDestroy()
	{
	}	
	
	#endregion
	
	#region Update Functions
		
	/// <summary>
	/// This updates this modifier using the default methods.  This can be overriden to implement custom triggering criteria.
	/// </summary>
	public virtual void Update () 
	{
		//make sure there is a target controller and it is enabled
		if (Controller != null && Controller.enabled)			
		{		
			//check if there is an active trigger delay timer
			if (m_fTriggerDelayTimer > 0.0f)
			{
				//update the trigger delay timer
				m_fTriggerDelayTimer -= Time.deltaTime;
			}
			//no delay timer is active, but make sure the trigger type is not none. 
			else if (triggerType != TriggerType.None)
			{
				//create bool to track if we should be triggered
				bool trigger = false;
				
				//update the trigger based on the current trigger type
				switch (triggerType)
				{
					case TriggerType.LessThanValueThreshold:
						if (Controller.GetCurrentValue() < triggerThreshold)
							trigger = true;
						break;
					case TriggerType.GreaterThanValueThreshold:
						if (Controller.GetCurrentValue() > triggerThreshold)
							trigger = true;
						break;
					case TriggerType.LessThanChangeThreshold:
						if (Controller.GetAdjustedValueDifference() < triggerThreshold)
							trigger = true;
						break;
					case TriggerType.GreaterThanChangeThreshold:
						if (Controller.GetAdjustedValueDifference() > triggerThreshold)
							trigger = true;
						break;
				}
				
				//check if we should trigger
				if (trigger)
				{
					//set the delay timer and call OnTrigger
					m_fTriggerDelayTimer = triggerReactivateDelay + UnityEngine.Random.Range(-triggerRandomReactivateDelay, triggerRandomReactivateDelay);
                    if (m_fTriggerDelayTimer < 0.0f)
                        m_fTriggerDelayTimer = 0.0f;

					OnTriggered(Controller.GetCurrentValue(),
		                        Controller.GetPreviousValue(),
		                        Controller.GetValueDifference(),
		                        Controller.GetAdjustedValueDifference());
				}
			}
		}
	}	
	
	/// <summary>
	/// This function is called by the trigger whenever 
	/// this trigger has been TRIGGERED.
	/// TO IMPLEMENT A CUSTOM TRIGGER REACTION, override this function 
	/// to handle the trigger event.
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
	public abstract void OnTriggered(float current, float previous, float difference, float adjustedDifference);
	
	#endregion
}
