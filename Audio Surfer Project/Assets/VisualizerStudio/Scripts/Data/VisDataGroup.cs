using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region Enumerations

/// <summary>
/// This enumeration defines what type of value a data group uses, i.e. how it is calculated. 
/// </summary>
public enum VisDataValueType 
{ 
	/// <summary>
	/// This indicates that the average of all values should be used. 
	/// </summary>
	Average = 0, 
	
	/// <summary>
	/// This indicates that the median/middle of all values should be used.
	/// </summary>
	Median, 
	
	/// <summary>
	/// This indicates that the sum of all values should be used.
	/// </summary>
	Sum, 
	
	/// <summary>
	/// This indicates that the minimum of all values should be used.
	/// </summary>
	Minimum, 
	
	/// <summary>
	/// This indicates that the maximum of all values should be used.
	/// </summary>
	Maximum 
};

/// <summary>
/// This enumeration indicates what the data source of a data group should be.
/// </summary>
public enum VisDataSource 
{ 
	/// <summary>
	/// This indicates that the raw audio wave data should be used.
	/// </summary>
	Raw = 0,
	
	/// <summary>
	/// This indicates that the spectrum data (the FFT result) should be used.
	/// </summary>
	Spectrum = 1 
};

#endregion

/// <summary>
/// This class encapsulates a section of the raw or spectrum data of the audio being played 
/// back in order to aggregate the data in that section in various ways. 
/// </summary>
[AddComponentMenu("Visualizer Studio/Data Group")]
public sealed class VisDataGroup : MonoBehaviour, IVisManagerTarget
{
	#region Defaults Static Class
	
	/// <summary>
	/// This internal class holds all of the defaults of the VisDataGroup class. 
	/// </summary>
	public static class Defaults
	{
        public static int dataGroupNameCounter = 0;
        public const string dataGroupName = "Default";
		public const VisDataSource dataSource = VisDataSource.Spectrum;
		public const int numberSubDataGroups = 5;
	    public const int frequencyRangeStartIndex = 0;
	    public const int frequencyRangeEndIndex = 256;		
		public const float boost = 1.0f;
	    public const float cutoff = 1.0f;	
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
            if (m_oVisManager != null)
                m_oVisManager.RemoveDataGroup(this);
            if (value != null)
                value.AddDataGroup(this);

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
	/// This is the name of this data group. 
	/// </summary>
	public string dataGroupName = "";
	
	/// <summary>
	/// This is the source of the data for this data group. 
	/// </summary>
	public VisDataSource dataSource = Defaults.dataSource;
	
	/// <summary>
	/// This indicates the number of times the target spectrum range 
	/// should be split up into small sub groups for aggregating the data. 
	/// </summary>
	public int numberSubDataGroups = Defaults.numberSubDataGroups;
	
	/// <summary>
	/// This is the start index of the target spectrum range of this data group. 
	/// </summary>
    public int frequencyRangeStartIndex = Defaults.frequencyRangeStartIndex;
	
	/// <summary>
	///	This is the end index of the target spectrum range of this data group. 
	/// </summary>
    public int frequencyRangeEndIndex = Defaults.frequencyRangeEndIndex;
	
	/// <summary>
	/// This is the scalar amount to apply to the data values of this data group. 
	/// </summary>
	public float boost = Defaults.boost;
	
	/// <summary>
	/// This is the maximum cutoff value to to the data values of this data group. 
	/// </summary>
    public float cutoff = Defaults.cutoff;
	
	/// <summary>
	/// This is the debug color to use for this data group. 
	/// </summary>
    public Color debugColor = Color.white;
	
	#endregion
	
	#region Private Member Variables
		
	/// <summary>
	/// This array contains a data container for each VisDataValueType.  It is split up this way so that
	/// it is possible to get the "Sum" of all sub data groups "Average".  Other examples would be 
	/// the following: "Median of the Sums", "Minimum of the Maximums", "Sum of the Sums", etc
	/// </summary>
	private VisDataContainer[] m_aoDataContainers = new VisDataContainer[(int)VisDataValueType.Maximum + 1];
	
	/// <summary>
	/// This list contains all of the sub data groups that define this data group.  The data
	/// group is split up into component parts in order to allow more granular aggregation
	/// of the visualization data.
	/// </summary>
	private List<VisSubDataGroup> m_oSubDataGroups = new List<VisSubDataGroup>();
	
	#endregion
	
	#region Properties
	
	#endregion
	
	#region Init/Deinit Functions

	/// <summary>
	/// This resets this data group to all default values. 
	/// </summary>
	public void Reset()
    {
        dataGroupName = Defaults.dataGroupName + (++Defaults.dataGroupNameCounter).ToString();

		dataSource = Defaults.dataSource;	
		numberSubDataGroups = Defaults.numberSubDataGroups;
	    frequencyRangeStartIndex = Defaults.frequencyRangeStartIndex;
	    frequencyRangeEndIndex = Defaults.frequencyRangeEndIndex;
		boost = Defaults.boost;
	    cutoff = Defaults.cutoff;		
	}
	
	/// <summary>
	/// This function is called when this data group is awoken, it is used to make sure it is registered with its target data group. 
	/// </summary>
	public void Awake()
	{
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
			Debug.LogError("This Data Group does not have a VisManager assigned to it, nor could it find an active VisManager. In order to function, this Data Group needs a VisManager!");
		}
	}
	
	/// <summary>
	/// This is called when this data group is started. 
	/// </summary>
	public void Start () 
	{		
		//validate the boost and cutoff for this data group.
		boost = VisHelper.Validate(boost, 0.001f, 10000.0f, Defaults.boost, this, "boost", false);
		cutoff = VisHelper.Validate(cutoff, 0.001f, 10000.0f, Defaults.cutoff, this, "cutoff", false);
		
		//create all data containers for all data value types
		for (int i = 0; i < m_aoDataContainers.Length; i++)
		{
			m_aoDataContainers[i] = new VisDataContainer();
		}
		
		//make sure there is a vis manager assigned to this data group
		if (m_oVisManager != null)
		{
			//validate the frequency values and number of sub data groups.
			frequencyRangeEndIndex = VisHelper.Validate(frequencyRangeEndIndex, 0, (int)m_oVisManager.windowSize, 0, this, "frequencyRangeEndIndex", true);
			frequencyRangeStartIndex = VisHelper.Validate(frequencyRangeStartIndex, 0, frequencyRangeEndIndex, 0, this, "frequencyRangeStartIndex", true);
			numberSubDataGroups = VisHelper.Validate(numberSubDataGroups, 1, (frequencyRangeEndIndex - frequencyRangeStartIndex) + 1, 1, this, "numberSubDataGroups", true);
								
			//create sub groups
			int range = (frequencyRangeEndIndex - frequencyRangeStartIndex) + 1;
			int increment = Mathf.RoundToInt(((float)range) / ((float)numberSubDataGroups));
			if (increment <= 0)
				increment = 1;
			int remainder = range - (increment * (range / increment));
			for (int i = frequencyRangeStartIndex; i <= frequencyRangeEndIndex - increment; i+=increment)
			{
				//get the start and end index and create the sub group
				int startIndex = i;
				int endIndex = i + increment - 1;
				m_oSubDataGroups.Add(new VisSubDataGroup(startIndex, endIndex));
			}
			
			//add the remainder onto the last sub group
			if (remainder > 0 && m_oSubDataGroups.Count > 0)
			{
				m_oSubDataGroups[m_oSubDataGroups.Count - 1].frequencyRangeEndIndex += remainder;
			}
		}
		else
		{
			//send warning that there is no vis manager.
			Debug.LogError("This data group must have an assigned VisManager.");
		}
	}
	
	/// <summary>
	/// This function is called when this data group is destroyed.  It makes sure that this data group is removed from its vis manager. 
	/// </summary>
	public void OnDestroy()
	{
		if (m_oVisManager != null)
			m_oVisManager.RemoveDataGroup(this);
	}

    /// <summary>
    /// This validates the manager for this data group, ensuring it is in the same game object as this one.
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
                Debug.LogWarning("This Data Group (" +
                                 dataGroupName +
                                 ") is in a different Game Object than it's Manager (" +
                                 m_oVisManager.name +
                                 ").  Please make sure it is attached to the same Game Object to prevent issues.", this);
            }

            return false;
        }
        return true;
    }

	/// <summary>
	/// This function makes sure that this data group is registered with its vis manager. 
	/// </summary>
	public void EnsureRegistered()
	{
		if (m_oVisManager != null)
			m_oVisManager.AddDataGroup(this);
	}
	
	#endregion
	
	#region Update Functions
	
	/// <summary>
	/// This is the main update function 
	/// </summary>
	void Update () 
	{
		//make sure there is a vis manager
		if (m_oVisManager != null)
		{
			//get the spectrum data
			float[] spectrum = dataSource == VisDataSource.Spectrum ? m_oVisManager.GetSpectrumData() : m_oVisManager.GetRawAudioData();
			
			//update all sub data groups
			for (int i = 0; i < m_oSubDataGroups.Count; i++)
			{
				m_oSubDataGroups[i].Update(spectrum);
			}
			
			//update all types of data containers
			for (int i = 0; i < m_aoDataContainers.Length; i++)
			{
				m_aoDataContainers[i].UpdatePreviousValues();
				m_aoDataContainers[i].ResetCurrentValues();
					
				//update and calculate current values
				for (int j = 0; j < m_oSubDataGroups.Count; j++)
				{
					//get the value
					float value = m_oSubDataGroups[j].GetValue((VisDataValueType)i);
					
					//add to sum
					m_aoDataContainers[i].sum += value;
									
					//update min/max
					if (Mathf.Abs(value) < m_aoDataContainers[i].minimum)
						m_aoDataContainers[i].minimum = value;
					if (Mathf.Abs(value) > m_aoDataContainers[i].maximum)
						m_aoDataContainers[i].maximum = value;
				}
				
				//calc average and median
				m_aoDataContainers[i].average = m_aoDataContainers[i].sum / (float)(m_oSubDataGroups.Count);
				m_aoDataContainers[i].median = (m_aoDataContainers[i].minimum + m_aoDataContainers[i].maximum) * 0.5f;
				
				//apply boost and cutoff and then update value differences
				m_aoDataContainers[i].ApplyBoostAndCutoff(boost, cutoff);				
				m_aoDataContainers[i].UpdateValueDifferences();
			}
		}
	}
	
	#endregion
	
	#region Accessor Functions
	
	/// <summary>
	/// This gets the value of this data group, based on the value types. 
	/// </summary>
	/// <param name="finalValueType">
	/// This is the value type of the final part of this value. i.e. 
	/// The maximum (finalValueType) of the average (subValueType) of all sub data groups.
	/// </param>
	/// <param name="subValueType">
	/// This is the value type of the sub data part of this value. i.e. 
	/// The maximum (finalValueType) of the average (subValueType) of all sub data groups.
	/// </param>
	/// <returns>
	/// This is the final value that was requested
	/// </returns>
	public float GetValue(VisDataValueType finalValueType, VisDataValueType subValueType)
	{
		return m_aoDataContainers[(int)subValueType].GetValue(finalValueType);
	}
		
	/// <summary>
	/// This gets the previous value of this data group, based on the value types. 
	/// </summary>
	/// <param name="finalValueType">
	/// This is the value type of the final part of this value. i.e. 
	/// The maximum (finalValueType) of the average (subValueType) of all sub data groups.
	/// </param>
	/// <param name="subValueType">
	/// This is the value type of the sub data part of this value. i.e. 
	/// The maximum (finalValueType) of the average (subValueType) of all sub data groups.
	/// </param>
	/// <returns>
	/// This is the final value that was requested
	/// </returns>
	public float GetPreviousValue(VisDataValueType finalValueType, VisDataValueType subValueType)
	{
		return m_aoDataContainers[(int)subValueType].GetPreviousValue(finalValueType);
	}
		
	/// <summary>
	/// This gets the value difference of this data group, based on the value types. 
	/// </summary>
	/// <param name="finalValueType">
	/// This is the value type of the final part of this value. i.e. 
	/// The maximum (finalValueType) of the average (subValueType) of all sub data groups.
	/// </param>
	/// <param name="subValueType">
	/// This is the value type of the sub data part of this value. i.e. 
	/// The maximum (finalValueType) of the average (subValueType) of all sub data groups.
	/// </param>
	/// <returns>
	/// This is the final value that was requested.
	/// </returns>
	public float GetValueDifference(VisDataValueType finalValueType, VisDataValueType subValueType)
	{
		return m_aoDataContainers[(int)subValueType].GetValueDifference(finalValueType);
	}
	
	#endregion
	
	#region Debug Functions
		
	/// <summary>
	/// This displays the debug information of this data group. 
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
	public Rect DisplayDebugGUI(int x, int y, int barWidth, int barHeight, int separation, Texture debugTexture)
	{
		//make sure there is a debug texture set
		if (debugTexture != null)
		{
			//calcuate initial vars
			int labelHeight = 20;
			int padding = 5;		
			int subFrameWidth = barWidth * m_oSubDataGroups.Count;
			int frameWidth = subFrameWidth * 5 + separation * 4 + padding * 2;
			int frameHeight = barHeight + padding * 2 + labelHeight * 4;	
			Rect frameRect = new Rect(x - padding, y - padding, frameWidth, frameHeight);
			
			//calc frequencies
			float startFreq = m_oVisManager != null ? (float)frequencyRangeStartIndex * m_oVisManager.FrequencyResolution : 0;
			float endFreq = m_oVisManager != null ? (float)(frequencyRangeEndIndex + 1) * m_oVisManager.FrequencyResolution : 0;
			
			//begin group and display labels
            GUI.BeginGroup(frameRect);
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(new Rect(0, 0, frameRect.width, frameRect.height), debugTexture);
            GUI.color = Color.white;
			GUI.Label(new Rect(padding, padding, 200, labelHeight + 3), "Data Group: \"" + dataGroupName + "\"");
			GUI.Label(new Rect(padding, padding + labelHeight, 200, labelHeight + 3), "Range: " + startFreq.ToString("F1") + " Hz to " + endFreq.ToString("F1") + " Hz");
			GUI.Label(new Rect(padding, padding + labelHeight * 2, 200, labelHeight + 3), "Boost: " + boost + "    Cutoff: " + cutoff);
			
			//display all data bars
			for (int valueType = 0; valueType <= (int)VisDataValueType.Maximum; valueType++)
			{				
				for (int i = 0; i < m_oSubDataGroups.Count; i++) 
				{
					float value = m_oSubDataGroups[i].GetValue((VisDataValueType)valueType);
					value *= boost;
					value = Mathf.Clamp(value, value < 0.0f ? -cutoff : 0.0f, value < 0.0f ? 0.0f : cutoff);
					float perc = (Mathf.Abs(value) / cutoff);
					//print(value);
					
					GUI.color = debugColor;
					
					if (dataSource == VisDataSource.Spectrum)
					{
						
						int height = (int)(((float)(barHeight - 1)) * perc) + 1;
						GUI.DrawTexture(new Rect(padding + barWidth * i + valueType * (subFrameWidth + separation), 
						                         labelHeight * 3 + padding + barHeight - height, 
						                         barWidth, height
						                         ), 
						                debugTexture);
					}
					else
					{						
						int height = (int)(((float)((barHeight / 2) - 1)) * perc) + 1;
						if (value < 0.0f)
							GUI.DrawTexture(new Rect(padding + barWidth * i + valueType * (subFrameWidth + separation), 
							                         labelHeight * 3 + padding + (barHeight / 2), 
							                         barWidth,
							                         height),
							                debugTexture);
						else
							GUI.DrawTexture(new Rect(padding + barWidth * i + valueType * (subFrameWidth + separation), 
							                         labelHeight * 3 + padding + (barHeight / 2) - height + 1, 
							                         barWidth, 
							                         height), 
							                debugTexture);
					}
				}
				
				string subLabel = "avg";
				switch ((VisDataValueType)valueType)
				{
					case VisDataValueType.Median:
						subLabel = "med";
						break;
					case VisDataValueType.Sum:
						subLabel = "sum";
						break;
					case VisDataValueType.Minimum:
						subLabel = "min";
						break;
					case VisDataValueType.Maximum:
						subLabel = "max";
						break;
				}
				GUI.color = Color.white;
				GUI.Label(new Rect(padding + valueType * (subFrameWidth + separation), padding + barHeight + labelHeight * 3, 200, labelHeight + 3), subLabel);
			}
				
			//draw border and end group
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
	/// This gets the string representation of this data group. 
	/// </summary>
	/// <returns>
	/// The string of this data group.
	/// </returns>
	public override string ToString ()
	{
		return "VisDataGroup \"" + dataGroupName + "\"";
	}

    #endregion

    #region Target Restore Functions

    /// <summary>
    /// This attempts to restore the last set data group on the target.
    /// </summary>
    /// <param name="target">The target to restore.</param>
    /// <returns>Whether or not the target was restored.</returns>
    public static bool RestoreVisDataGroupTarget(IVisDataGroupTarget target)
    {
        //make sure the data group is set, and if not, check if there 
        //is a name set and try and find that object as the data group
        if (target.DataGroup == null && target.LastDataGroupName != null && target.LastDataGroupName.Length > 0)
        {
            //try to get the manager for this target
            VisManager manager = null;
            if (target is IVisManagerTarget)
                manager = (target as IVisManagerTarget).Manager;

            //make sure a manager was found
            if (manager != null)
            {
                //loop through all data groups and make sure it was found
                for (int i = 0; i < manager.DataGroups.Count; i++)
                {
                    if (manager.DataGroups[i].dataGroupName == target.LastDataGroupName)
                    {
                        target.DataGroup = manager.DataGroups[i];
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
/// This interface is used to mark a class as being able to target a data group. 
/// </summary>
public interface IVisDataGroupTarget
{
	/// <summary>
	/// This gets/sets the data group. 
	/// </summary>
	VisDataGroup DataGroup
	{
		get;
		set;
    }

    /// <summary>
    /// This gets the name of the last data group that was set to this target.
    /// </summary>
    string LastDataGroupName
    {
        get;
    }
}