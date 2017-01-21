using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is the base class for all controllers.  Controllers are used to pull a single scalar 
/// piece of data from a data group, to be used to modify or trigger objects in the scene. 
/// </summary>
public abstract class VisBaseController : MonoBehaviour, IVisManagerTarget
{		
	#region Defaults Static Class
	
	/// <summary>
    /// This internal class holds all of the defaults of the VisBaseController class. 
	/// </summary>
	public static class Defaults
	{
        public static int controllerNameCounter = 0;
        public const string controllerName = "Default";
		public const bool limitIncreaseRate = false;
		public const float increaseRate = 1.0f;	
		public const bool limitDecreaseRate = true;
		public const float decreaseRate = 1.0f;
	}
	
	#endregion
	
	#region Constants
	
	/// <summary>
	/// This is the amount of time that the input values should act
	/// as if they changed in.  For instance, if the value went up 
	/// 0.1f in 0.05f seconds, the following would be the used value: 
	/// adjustedChange = ((1.0f / 0.05f) * 0.1f) / mc_fTargetAdjustedDifferenceTime 
	/// </summary>
	public const float mc_fTargetAdjustedDifferenceTime = 1.0f / 60.0f;

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
            if (m_oVisManager != null)
                m_oVisManager.RemoveController(this);
            if (value != null)
                value.AddController(this);

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
	
	#region Public Member Variables
	
	/// <summary>
	/// This is the name of this controller. 
	/// </summary>
    //[HideInInspector()]
	public string controllerName = ""; 	
	
	/// <summary>
	/// This indicates if the increase rate of this controllers values should be limited. 
    /// </summary>
    //[HideInInspector()]
	public bool limitIncreaseRate = Defaults.limitIncreaseRate;
	
	/// <summary>
	/// This is the rate at which the value of this controller can increase per second. 
    /// </summary>
    //[HideInInspector()]
	public float increaseRate = Defaults.increaseRate;
		
	/// <summary>
	/// This indicates if the decrease rate of this controllers values should be limited. 
    /// </summary>
    //[HideInInspector()]
	public bool limitDecreaseRate = Defaults.limitDecreaseRate;
	
	/// <summary>
	/// This is the rate at which the value of this controller can decrease per second. 
    /// </summary>
    //[HideInInspector()]
	public float decreaseRate = Defaults.decreaseRate;
	
	#endregion
	
	#region Protected Member Variables
	
	/// <summary>
	/// This is the value of this controller from the previous frame. 
	/// </summary>
	protected float m_fPreviousValue = 0.0f;
	
	/// <summary>
	/// This is the current value of this controller. 
	/// </summary>
	protected float m_fValue = 0.0f;
	
	/// <summary>
	/// This is the difference of the current value and the previous value. 
	/// </summary>
	protected float m_fValueDifference = 0.0f;
	
	/// <summary>
	/// This is the ADJUSTED difference of the current value and the previous value. 
	/// The adjusted value is the change of the value as if it took place over a 
	/// certain time period, controlled by mc_fTargetAdjustedDifferenceTime.  The 
	/// default of this essientially indicates a frame rate of 60 fps to determine 
	/// the adjusted difference.  This should be used for almost all difference 
	/// calculations, as it is NOT frame rate dependent.
	/// </summary>
	protected float m_fAdjustedValueDifference = 0.0f;
	
	#endregion
	
	#region Private Member Variables	
	
	/// <summary>
	/// This is the minimum value that this controller has ever been set to. 
	/// </summary>
	private float m_fMinValue = 0.0f;
	
	/// <summary>
	/// This is the maximum value that this controller has ever beens set to. 
	/// </summary>
	private float m_fMaxValue = 1.0f;
	
	#endregion
	
	#region Properties
		
	/// <summary>
	/// This gets the minimum value that this controller has ever been set to. 
	/// </summary>
	public float MinValue
	{
		get { return m_fMinValue; }
	}
			
	/// <summary>
	/// This gets the maximum value that this controller has ever been set to. 
	/// </summary>
	public float MaxValue
	{
		get { return m_fMaxValue; }
	}
	
	#endregion
	
	#region Init/Deinit Functions
	
	/// <summary>
	/// This function resets this controller to default values 
	/// </summary>
	public virtual void Reset()
    {
        controllerName = Defaults.controllerName + (++Defaults.controllerNameCounter).ToString();

		limitIncreaseRate = Defaults.limitIncreaseRate;
		increaseRate = Defaults.increaseRate;		
		limitDecreaseRate = Defaults.limitDecreaseRate;
		decreaseRate = Defaults.decreaseRate;
	}
	
	/// <summary>
	/// This is called when this component is woken up. 
	/// </summary>
	public virtual void Awake()
    {
        //make sure to restore the targets if needed
        VisManager.RestoreVisManagerTarget(this);

		//check if there is already a vis manager assigned to this data group
		if (m_oVisManager == null)
		{
			//try to grab a vis manager that belongs to this game object.
			m_oVisManager = GetComponent<VisManager>();
			
			//check if a vis manager was found
			if (m_oVisManager == null)
			{
				//find all game objects with vis managers
				Object[] visManagers = GameObject.FindObjectsOfType(typeof(VisManager));
				for (int i = 0; i < visManagers.Length; i++)
				{
					//get this manager and check if it is enabled
					VisManager manager = visManagers[i] as VisManager;
					if (manager.enabled)
					{
						//assign this vis manager
						m_oVisManager = manager;
						break;
					}
				}
			}
        }

        //validate manager
        ValidateManager(true);
	
		//make sure this data group is registered with it vis manager
		EnsureRegistered();
		
		//log an error if no vis manager is assigned
		if (m_oVisManager == null)
		{
			Debug.LogError("This Controller does not have a VisManager assigned to it, nor could it find an active VisManager. In order to function, this Controller needs a VisManager!");
		}
	}
	
	/// <summary>
	/// The main start function.
	/// </summary>
	public virtual void Start()
    {		
		increaseRate = VisHelper.Validate(increaseRate, 0.00001f, 10000.0f, Defaults.increaseRate, this, "increaseRate", false);
		decreaseRate = VisHelper.Validate(decreaseRate, 0.00001f, 10000.0f, Defaults.decreaseRate, this, "decreaseRate", false);	
	}
	
	/// <summary>
	/// This is called when this controller is destroyed. 
	/// </summary>
	public virtual void OnDestroy()
	{
		if (m_oVisManager != null)
			m_oVisManager.RemoveController(this);
	}

    /// <summary>
    /// This validates the manager for this controller, ensuring it is in the same game object as this one.
    /// </summary>
    /// <param name="displayWarning">Indicates if a warning should be displayed.</param>
    /// <returns>Returns whether or not the manager is valid</returns>
    public bool ValidateManager(bool displayWarning)
    {
        if (m_oVisManager != null &&
            m_oVisManager.gameObject != this.gameObject)
        {
            if (displayWarning)
            {
                Debug.LogWarning("This Controller (" +
                                 controllerName +
                                 ") is in a different Game Object than it's Manager (" +
                                 m_oVisManager.name +
                                 ").  Please make sure it is attached to the same Game Object to prevent issues.", this);
            }
            return false;
        }
        return true;
    }
		
	/// <summary>
	/// This function makes sure that this controller is registered with its vis manager. 
	/// </summary>
	public void EnsureRegistered()
	{
		if (m_oVisManager != null)
			m_oVisManager.AddController(this);
	}
	
	#endregion
	
	#region Update Functions
	
	/// <summary>
	/// This updates this controller.  This should not be overridden!  To implement custom controller functionality, override GetCustomControllerValue().
	/// </summary>
	public void Update() 
	{
		//set previous value
		m_fPreviousValue = m_fValue;
		
		//get the target value from the custom controller.
		float targetValue = GetCustomControllerValue();	
		
		//aproach target value
		if (targetValue < m_fValue && limitDecreaseRate)
			m_fValue = Mathf.MoveTowards(m_fValue, targetValue, decreaseRate * Time.deltaTime);
		else if (targetValue > m_fValue && limitIncreaseRate)
			m_fValue = Mathf.MoveTowards(m_fValue, targetValue, increaseRate * Time.deltaTime);
		else
			m_fValue = targetValue;
		
		//update value difference
		m_fValueDifference = m_fValue - m_fPreviousValue;
		
		//calculate adjusted value difference
		if (Mathf.Abs(m_fValueDifference) <= float.Epsilon)
			m_fAdjustedValueDifference = 0.0f;
		else
			m_fAdjustedValueDifference = ((1.0f / Mathf.Clamp(Time.deltaTime, 0.0001f, 0.5f)) * m_fValueDifference) / (1.0f / mc_fTargetAdjustedDifferenceTime);
		
		//update min/max values
		if (m_fValue < m_fMinValue)
			m_fMinValue = m_fValue;
		if (m_fValue > m_fMaxValue)
			m_fMaxValue = m_fValue;
	}
	
	#endregion
	
	#region Accessor Functions
	
	/// <summary>
	/// This function returns the current value for this controller.
	/// TO IMPLEMENT A CUSTOM CONTROLLER, override this function 
	/// to return the current target value.
	/// </summary>
	/// <returns>
	/// The custom controller value.
	/// </returns>
	public virtual float GetCustomControllerValue() { return 0.0f; }
	
	/// <summary>
	/// This gets the current value of this controller. 
	/// </summary>
	/// <returns>
	/// The current value.
	/// </returns>
	public float GetCurrentValue() { return m_fValue; }
	
	/// <summary>
	/// This gets the previous value of this controller. 
	/// </summary>
	/// <returns>
	/// The previous value.
	/// </returns>
	public float GetPreviousValue() { return m_fPreviousValue; }
	
	/// <summary>
	/// This gets the value difference of this controller. 
	/// </summary>
	/// <returns>
	/// The value difference.
	/// </returns>
	public float GetValueDifference() { return m_fValueDifference; }
	
	/// <summary>
	/// This gets the adjusted value difference of this controller. 
	/// </summary>
	/// <returns>
	/// The adjusted value difference.
	/// </returns>
	public float GetAdjustedValueDifference() { return m_fAdjustedValueDifference; }
	
	#endregion
	
	#region Debug Functions
			
	/// <summary>
	/// This displays the debug information of this controller. 
	/// </summary>
	/// <param name="x">
	/// The x location to display this data group.
	/// </param>
	/// <param name="y">
	/// The y location to display this data group.
	/// </param>
	/// <param name="barWidth">
	/// This is the width in pixels of the debug bars.
	/// </param>
	/// <param name="barHeight">
	/// This is the height in pixels of the debug bars.
	/// </param>
	/// <param name="separation">
	/// This is the separation in pixels of the debug bars.
	/// </param>
	/// <param name="debugTexture">
	/// This is the texture used to display the debug information.
	/// </param>
	/// <returns>
	/// This is the rect of the of the debug information that was displayed.
	/// </returns>
	public virtual Rect DisplayDebugGUI(int x, int y, int barWidth, int barHeight, int separation, Texture debugTexture)
	{
		//make sure there is a debug texture set
		if (debugTexture != null)
		{
			//calcuate initial vars
			int labelWidth = 150;
			int labelHeight = 20;
			int padding = 5;		
			int frameWidth = Mathf.Max(barWidth, labelWidth) + padding * 2;
			int frameHeight = padding * 2 + labelHeight * 2 + barHeight;	
			Rect frameRect = new Rect(x - padding, y - padding, frameWidth, frameHeight);
			
			//begin group and display labels
			GUI.BeginGroup(frameRect);
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(new Rect(0, 0, frameRect.width, frameRect.height), debugTexture);
            GUI.color = Color.white;
			GUI.Label(new Rect(padding, padding, labelWidth, labelHeight + 3), "Controller: \"" + controllerName + "\"");
			GUI.Label(new Rect(padding, padding + labelHeight, labelWidth, labelHeight + 3), "VALUE: " + GetCurrentValue().ToString("F4"));
			
			//draw data bar
			float perc = ((m_fValue - m_fMinValue) / (m_fMaxValue - m_fMinValue)) * 0.975f + 0.025f;
			GUI.DrawTexture(new Rect(padding, padding + labelHeight * 2, (int)(((float)barWidth)*perc), barHeight), debugTexture);
				
			//draw frame and end group
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(0, 0, frameWidth, 1), debugTexture);
			GUI.DrawTexture(new Rect(0, frameHeight - 1, frameWidth, 1), debugTexture);
			GUI.DrawTexture(new Rect(0, 0, 1, frameHeight), debugTexture);
			GUI.DrawTexture(new Rect(frameWidth - 1, 0, 1, frameHeight), debugTexture);
			GUI.EndGroup();		
			
			//return the rect of the frame that was drawn
			return frameRect;
		}
		return new Rect(0,0,0,0);
	}
	
	#endregion
	
	#region Object Class Functions
	
	/// <summary>
	/// This gets the string representation of this controller. 
	/// </summary>
	/// <returns>
	/// The string of this data group.
	/// </returns>
	public override string ToString ()
	{
		return "VisBaseController \"" + controllerName + "\"";
	}

    #endregion

    #region Target Restore Functions

    /// <summary>
    /// This attempts to restore the last set controller on the target.
    /// </summary>
    /// <param name="target">The target to restore.</param>
    /// <returns>Whether or not the target was restored.</returns>
    public static bool RestoreVisBaseControllerTarget(IVisBaseControllerTarget target)
    {
        //make sure the controller is set, and if not, check if there 
        //is a name set and try and find that object as the controller
        if (target.Controller == null && target.LastControllerName != null && target.LastControllerName.Length > 0)
        {
            //try to get the manager for this target
            VisManager manager = null;
            if (target is IVisManagerTarget)
                manager = (target as IVisManagerTarget).Manager;

            //make sure a manager was found
            if (manager != null)
            {
                //loop through all controllers and make sure it was found
                for (int i = 0; i < manager.Controllers.Count; i++)
                {
                    if (manager.Controllers[i].controllerName == target.LastControllerName)
                    {
                        target.Controller = manager.Controllers[i];
                        return true;
                    }
                }
            }
        }

        return false;
    }

    #endregion
}

/// <summary>
/// This interface is used to mark a class as being able to target a controller. 
/// </summary>
public interface IVisBaseControllerTarget
{
	/// <summary>
	/// This gets/sets the controller. 
	/// </summary>
	VisBaseController Controller
	{
		get;
		set;
    }

    /// <summary>
    /// This gets the name of the last controller that was set to this target.
    /// </summary>
    string LastControllerName
    {
        get;
    }
}