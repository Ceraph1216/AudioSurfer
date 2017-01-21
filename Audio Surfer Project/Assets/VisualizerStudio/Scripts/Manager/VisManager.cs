using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// This class is the master object that controls all of the base visualization functionality. 
/// </summary>
[AddComponentMenu("Visualizer Studio/Manager")]
public sealed class VisManager : MonoBehaviour 
{
	#region Defaults Static Class
	
	/// <summary>
	/// This internal class holds all of the defaults of the VisManager class. 
	/// </summary>
	public static class Defaults
	{		
		public const Channel channel = Channel.Left;
		public const WindowSize windowSize = WindowSize._1024;
		public const FFTWindow windowType = FFTWindow.Hamming;
		public const bool useAudioListener = false;
		public const bool displaySpectrumDebug = false;
		public const bool displayDataGroupDebug = false;
		public const bool displayControllerDebug = false;
		public const int debugSpectrumBarWidth = 1;
		public const int debugSpectrumBarHeight = 80;
		public const int debugRawAudioBarHeight = 40;
		public const int debugDataGroupBarWidth = 4;
		public const int debugDataGroupBarHeight = 40;
		public const int debugControllerBarWidth = 150;	
		public const int debugControllerBarHeight = 6;
		public const int debugSeparation = 5;
	}
	
	#endregion
	
	#region Enumerations
	
	/// <summary>
	/// This enumeration describes the target window size to use for running the FFT. 
	/// </summary>
	public enum WindowSize 
    { 
        /// <summary>
        /// Indicates a window size of 64.
        /// </summary>
        _64 = 64,

        /// <summary>
        /// Indicates a window size of 128.
        /// </summary>
        _128 = 128,

        /// <summary>
        /// Indicates a window size of 256.
        /// </summary>
        _256 = 256,

        /// <summary>
        /// Indicates a window size of 512.
        /// </summary>
        _512 = 512,

        /// <summary>
        /// Indicates a window size of 1024.
        /// </summary>
        _1024 = 1024,

        /// <summary>
        /// Indicates a window size of 2048.
        /// </summary>
        _2048 = 2048,

        /// <summary>
        /// Indicates a window size of 4096.
        /// </summary>
        _4096 = 4096,

        /// <summary>
        /// Indicates a window size of 8192.
        /// </summary> 
        _8192 = 8192 
    } 
	
	/// <summary>
	/// This enumeration describes the target channel to run the FFT on.
	/// </summary>
	public enum Channel 
    { 
        /// <summary>
        /// Indicates the left audio channel.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Indicates the right audio channel.
        /// </summary>
        Right = 1,

        /// <summary>
        /// Indicates the average of the left and right audio channels.
        /// </summary>
        Average,

        /// <summary>
        /// Indicates the minimum of the left and right audio channels.
        /// </summary>
        Min,

        /// <summary>
        /// Indicates the maximum of the left and right audio channels.
        /// </summary>
        Max 
    }
	
	#endregion
	
	#region Public Variables
	
	/// <summary>
	/// This is the audio channel to run the FFT on. 
	/// </summary>
	public Channel channel = Defaults.channel;
	
	/// <summary>
	/// This is the windows size to use when running the FFT. 
	/// </summary>
	public WindowSize windowSize = Defaults.windowSize;
	
	/// <summary>
	/// This is the type of FFT window to run on the audio. 
	/// </summary>
	public FFTWindow windowType = Defaults.windowType;

    /// <summary>
    /// This is the current audio source that is being used as the input audio signal. 
    /// </summary>
    public AudioSource audioSource = null;
		 
	/// <summary>
	/// This indicates if the debugging information for the spectrum should be displayed. 
	/// </summary>
	public bool displaySpectrumDebug = Defaults.displaySpectrumDebug;
		 
	/// <summary>
	/// This indicates if the debugging information for the data groups should be displayed. 
	/// </summary>
	public bool displayDataGroupDebug = Defaults.displayDataGroupDebug;
		 
	/// <summary>
	/// This indicates if the debugging information for the controllers should be displayed. 
	/// </summary>
	public bool displayControllerDebug = Defaults.displayControllerDebug;
	
	/// <summary>
	/// This is the pixel width to draw the spectrum bars at. 
	/// </summary>
	public int debugSpectrumBarWidth = Defaults.debugSpectrumBarWidth;
	
	/// <summary>
	/// This is the maximum pixel height to draw the spectrum bars at. 
	/// </summary>
	public int debugSpectrumBarHeight = Defaults.debugSpectrumBarHeight;
	
	/// <summary>
	/// This is the maximum pixel height to draw the raw audio bars at. 
	/// </summary>
	public int debugRawAudioBarHeight = Defaults.debugRawAudioBarHeight;
	
	/// <summary>
	/// This is the pixel width to draw the data group bars at. 
	/// </summary>
	public int debugDataGroupBarWidth = Defaults.debugDataGroupBarWidth;
	
	/// <summary>
	/// This is the maximum pixel height to draw the data group bars at. 
	/// </summary>
	public int debugDataGroupBarHeight = Defaults.debugDataGroupBarHeight;
	
	/// <summary>
    /// This is the maximum bar width to draw the controller bars at. 
	/// </summary>
	public int debugControllerBarWidth = Defaults.debugControllerBarWidth;
	
	/// <summary>
	/// This is the pixel height to draw the controller bars at. 
	/// </summary>
	public int debugControllerBarHeight = Defaults.debugControllerBarHeight;
	
	/// <summary>
	/// This is the pixels of separation to draw the debug windows at. 
	/// </summary>
	public int debugSeparation = 5;
	
	/// <summary>
	/// This is the debug texture to use when drawing the debug frames.  This should just be a plain white texture. 
	/// </summary>
	public Texture debugTexture = null;
	
	#endregion
	
	#region Private Member Variables
	
	/// <summary>
	/// This is the array that holds all of the spectrum data for the current audio input.  (This is the FFT results) 
	/// </summary>
    private float[] m_afSpectrumData = null;
	
	/// <summary>
	/// This is the array that holds all of the raw audio data for the current audio input.  (This is raw wave form) 
	/// </summary>
    private float[] m_afRawAudioData = null;
	
	/// <summary>
	/// This is the array that holds the auxiliary audio data, used as temporary storage for merging left and right audio stream data.
	/// </summary>
	private float[] m_afAuxiliaryData = null;
	
	/// <summary>
	/// This is the list of data groups that are registered and active with this VisManager. 
	/// </summary>
    private List<VisDataGroup> m_oDataGroups = new List<VisDataGroup>();
	
	/// <summary>
	/// This is the list of controllers that are registered and active with this VisManager. 
	/// </summary>
	private List<VisBaseController> m_oControllers = new List<VisBaseController>();
	
	/// <summary>
	/// This is the max displayed debug value. 
	/// </summary>
	private float m_fMaxDebugValue = 0.1f;
	
	/// <summary>
	/// This is the range of the song frequency in Hz. 
	/// </summary>
	private int m_nFrequencyRange = 0;
	
	/// <summary>
	/// This is the size of each sample in Hz. 
	/// </summary>
    private float m_fFrequencyResolution = 0;

    /// <summary>
    /// This indicates whether or not this VisManager should use the static AudioListener. 
    /// </summary>
    private bool m_bUseAudioListener = Defaults.useAudioListener;
	
	#endregion
	
	#region Properties
	
	/// <summary>
	/// This gets the read only list of all data groups in this VisManager. 
	/// </summary>
	public ReadOnlyCollection<VisDataGroup> DataGroups
	{
		get 
		{
			if (m_oDataGroups == null) 	
				m_oDataGroups = new List<VisDataGroup>();
			return m_oDataGroups.AsReadOnly(); 
		}
	}
		
	/// <summary>
	/// This gets the read only list of all controllers in this VisManager. 
	/// </summary>
	public ReadOnlyCollection<VisBaseController> Controllers
	{
		get 
		{
			if (m_oControllers == null) 	
				m_oControllers = new List<VisBaseController>();
			return m_oControllers.AsReadOnly(); 
		}
	}
	
	/// <summary>
	/// This gets the size of each sample in Hz. 
	/// </summary>
	public float FrequencyResolution
	{
		get { return m_fFrequencyResolution; }
	}
	
	#endregion
	
	#region Init Functions
	
    /// <summary>
    /// This is called when the vis manager is reset.
    /// </summary>
	public void Reset()
	{
		channel = Defaults.channel;
		windowSize = Defaults.windowSize;
		windowType = Defaults.windowType;
		
		m_bUseAudioListener = Defaults.useAudioListener;
		displaySpectrumDebug = Defaults.displaySpectrumDebug;
		displayDataGroupDebug = Defaults.displayDataGroupDebug;
		displayControllerDebug = Defaults.displayControllerDebug;
		debugSpectrumBarWidth = Defaults.debugSpectrumBarWidth;
		debugSpectrumBarHeight = Defaults.debugSpectrumBarHeight;
		debugRawAudioBarHeight = Defaults.debugRawAudioBarHeight;
		debugDataGroupBarWidth = Defaults.debugDataGroupBarWidth;
		debugDataGroupBarHeight = Defaults.debugDataGroupBarHeight;
		debugControllerBarWidth = Defaults.debugControllerBarWidth;
		debugControllerBarHeight = Defaults.debugControllerBarHeight;
		debugSeparation = Defaults.debugSeparation;
        debugTexture = null;
	}
	
	/// <summary>
	/// This function sets up this VisManager. 
	/// </summary>
	public void Start () 
	{	
		//validate debug variables
		debugSpectrumBarWidth = VisHelper.Validate(debugSpectrumBarWidth, 1, 1000, Defaults.debugSpectrumBarWidth, this, "debugSpectrumBarWidth", false);
		debugSpectrumBarHeight = VisHelper.Validate(debugSpectrumBarHeight, 1, 1000, Defaults.debugSpectrumBarHeight, this, "debugSpectrumBarHeight", false);
		debugRawAudioBarHeight = VisHelper.Validate(debugRawAudioBarHeight, 1, 1000, Defaults.debugRawAudioBarHeight, this, "debugRawAudioBarHeight", false);
		debugDataGroupBarWidth = VisHelper.Validate(debugDataGroupBarWidth, 1, 1000, Defaults.debugDataGroupBarWidth, this, "debugDataGroupBarWidth", false);
		debugDataGroupBarHeight = VisHelper.Validate(debugDataGroupBarHeight, 1, 1000, Defaults.debugDataGroupBarHeight, this, "debugDataGroupBarHeight", false);
		debugControllerBarWidth = VisHelper.Validate(debugControllerBarWidth, 1, 1000, Defaults.debugControllerBarWidth, this, "debugControllerBarWidth", false);
		debugControllerBarHeight = VisHelper.Validate(debugControllerBarHeight, 1, 1000, Defaults.debugControllerBarHeight, this, "debugControllerBarHeight", false);
		debugSeparation = VisHelper.Validate(debugSeparation, 0, 1000, Defaults.debugSeparation, this, "debugSeparation", false);
		
		//Create the array to hold the audio data and initialize them
		m_afSpectrumData = new float[(int)windowSize];
		m_afRawAudioData = new float[(int)windowSize];
		m_afAuxiliaryData = new float[(int)windowSize];
		for (int i = 0; i < (int)windowSize; i++) {
			m_afSpectrumData[i] = 0.0f;
			m_afRawAudioData[i] = 0.0f;
			m_afAuxiliaryData[i] = 0.0f;
		}

        //try to load 1x1white texture
        if (debugTexture == null)
            debugTexture = (Texture)Resources.Load("1x1White");
		
		//Display a warning if no audio source was found.
		if (audioSource == null)
		{
			Debug.LogWarning("A VisManager should have an Audio Source assigned to it so it only listens to the specified music. " +
                             "This manager will now default to using the static Audio Listener, which contains ALL audio playing " + 
                             "in the game.  This Audio Listener does not work near as well as listening directly to an Audio Source, " +
                             "so it is NOT recommended.");
            m_bUseAudioListener = true;
		}
		
		CalculateFrequencyResolution();
	}

    /// <summary>
    /// This function should be used to set the audio source at runtime.  This will call Start() to reset the
    /// VisManager after calling.
    /// </summary>
    /// <param name="_audioSource">The audio source to use for this VisManager.</param>
    public void SetAudioSource(AudioSource _audioSource)
    {
        audioSource = _audioSource;
        Start();
    }
	
	/// <summary>
	/// This calculates the current frequency range and resolution for the playing audio clip. 
	/// </summary>
	public void CalculateFrequencyResolution()
	{
		if (!m_bUseAudioListener && audioSource != null && audioSource.clip != null)
		{
			m_nFrequencyRange = audioSource.clip.frequency;
			m_fFrequencyResolution = (float)m_nFrequencyRange / (float)windowSize;
		}
		else
		{
			m_fFrequencyResolution = 0;
		}
	}
	
	#endregion
	
	#region Update Functions
	
	/// <summary>
	/// This function updates this VisManager and runs the FFT on the target audio input,
	/// as well, it grabs the raw audio data.
	/// </summary>
	void Update () 
	{
		//Check if the audio listener should be used
		if (m_bUseAudioListener)
		{			
			//check if only the left or right channel was requested
			if (channel == VisManager.Channel.Left || channel == VisManager.Channel.Right)
			{
				//get the fft spectrum and raw audio data
				AudioListener.GetSpectrumData(m_afSpectrumData, (int)channel, windowType);
				AudioListener.GetOutputData(m_afRawAudioData, (int)channel);
			}
			//both channels were requested
			else
			{
				//get the fft spectrum data, from left and right channels, and merge the result
				AudioListener.GetSpectrumData(m_afSpectrumData, 0, windowType);
				AudioListener.GetSpectrumData(m_afAuxiliaryData, 1, windowType);
				for (int i = 0; i < (int)windowSize; i++) 
				{
					if (channel == VisManager.Channel.Average)
						m_afSpectrumData[i] = (m_afSpectrumData[i] + m_afAuxiliaryData[i]) * 0.5f;
					else if (channel == VisManager.Channel.Min)
						m_afSpectrumData[i] = Mathf.Min(m_afSpectrumData[i], m_afAuxiliaryData[i]);
					else if (channel == VisManager.Channel.Max)
						m_afSpectrumData[i] = Mathf.Max(m_afSpectrumData[i], m_afAuxiliaryData[i]);
				}
				
				//get the raw audio data, from left and right channels, and merge the result
				AudioListener.GetOutputData(m_afRawAudioData, 0);
				AudioListener.GetOutputData(m_afAuxiliaryData, 1);
				for (int i = 0; i < (int)windowSize; i++) 
				{
					if (channel == VisManager.Channel.Average)
						m_afRawAudioData[i] = (m_afRawAudioData[i] + m_afAuxiliaryData[i]) * 0.5f;
					else if (channel == VisManager.Channel.Min)
					{
						if (Mathf.Abs(m_afAuxiliaryData[i]) < Mathf.Abs(m_afRawAudioData[i]))
							m_afRawAudioData[i] = m_afAuxiliaryData[i];
					}
					else if (channel == VisManager.Channel.Max)
					{						
						if (Mathf.Abs(m_afAuxiliaryData[i]) > Mathf.Abs(m_afRawAudioData[i]))
							m_afRawAudioData[i] = m_afAuxiliaryData[i];
					}
				}
			}
		}
		//Check if there is an audio source set to this VisManager
		else if (audioSource != null)
		{			
			//check if only the left or right channel was requested
			if (channel == VisManager.Channel.Left || channel == VisManager.Channel.Right)
			{
				//get the fft spectrum and raw audio data
				audioSource.GetSpectrumData(m_afSpectrumData, (int)channel, windowType);
				audioSource.GetOutputData(m_afRawAudioData, (int)channel);
			}
			//both channels were requested
			else
			{
				//get the fft spectrum data, from left and right channels, and merge the result
				audioSource.GetSpectrumData(m_afSpectrumData, 0, windowType);
				audioSource.GetSpectrumData(m_afAuxiliaryData, 1, windowType);
				for (int i = 0; i < (int)windowSize; i++) 
				{
					if (channel == VisManager.Channel.Average)
						m_afSpectrumData[i] = (m_afSpectrumData[i] + m_afAuxiliaryData[i]) * 0.5f;
					else if (channel == VisManager.Channel.Min)
						m_afSpectrumData[i] = Mathf.Min(m_afSpectrumData[i], m_afAuxiliaryData[i]);
					else if (channel == VisManager.Channel.Max)
						m_afSpectrumData[i] = Mathf.Max(m_afSpectrumData[i], m_afAuxiliaryData[i]);
				}
				
				//get the raw audio data, from left and right channels, and merge the result
				audioSource.GetOutputData(m_afRawAudioData, 0);
				audioSource.GetOutputData(m_afAuxiliaryData, 1);
				for (int i = 0; i < (int)windowSize; i++) 
				{
					if (channel == VisManager.Channel.Average)
						m_afRawAudioData[i] = (m_afRawAudioData[i] + m_afAuxiliaryData[i]) * 0.5f;
					else if (channel == VisManager.Channel.Min)
					{
						if (Mathf.Abs(m_afAuxiliaryData[i]) < Mathf.Abs(m_afRawAudioData[i]))
							m_afRawAudioData[i] = m_afAuxiliaryData[i];
					}
					else if (channel == VisManager.Channel.Max)
					{						
						if (Mathf.Abs(m_afAuxiliaryData[i]) > Mathf.Abs(m_afRawAudioData[i]))
							m_afRawAudioData[i] = m_afAuxiliaryData[i];
					}
				}
			}
		}		
		
		//update the max debug value
		if (displaySpectrumDebug && debugTexture != null)
		{
			m_fMaxDebugValue -= 0.05f * Time.deltaTime;
			if (m_fMaxDebugValue < 0.1f)
				m_fMaxDebugValue = 0.1f;
		}
	}
	
	#endregion
	
	#region Accessor Functions

	/// <summary>
	/// This adds a data group to this VisManager. 
	/// </summary>
	/// <param name="dataGroup">
	/// This is the data group to add.
	/// </param>
    public void AddDataGroup(VisDataGroup dataGroup)
    {
		//Make sure the data group is not null, and it is not already in this VisManager.
        if (dataGroup != null && m_oDataGroups != null && m_oDataGroups.Contains(dataGroup) == false &&
            (dataGroup.Manager != this || GetDataGroupByName(dataGroup.dataGroupName) == null))
        {
            //make sure there is not a controller already in here with the game object name and controller name the same
            for (int i = 0; i < m_oDataGroups.Count; i++)
            {
                if (m_oDataGroups[i].name == dataGroup.name &&
                    m_oDataGroups[i].dataGroupName == dataGroup.dataGroupName)
                {
                    //this object may already be in there, ignore it!
                    return;
                }
            }

            //force unique name
            dataGroup.dataGroupName = EnsureUniqueDataGroupName(dataGroup.dataGroupName);

			//add the data group
            m_oDataGroups.Add(dataGroup);
            
            //check for data group overlap and warn
            for (int i = 0; i < m_oDataGroups.Count; i++)
            {
				//make sure the data group at this index is not the one we just added
                if (m_oDataGroups[i] != dataGroup &&
                    m_oDataGroups[i] != null)
                {
					//create var to store if there is an overlap
					bool overlapDetected = false;
					
					//check for an overlap with the of the start index of the just added data group.
                    if (dataGroup.frequencyRangeStartIndex >= m_oDataGroups[i].frequencyRangeStartIndex &&
                        dataGroup.frequencyRangeStartIndex <= m_oDataGroups[i].frequencyRangeEndIndex)
                    {
						overlapDetected = true;
                    }
					//check for an overlap with the of the end index of the just added data group.
                    else if (dataGroup.frequencyRangeEndIndex >= m_oDataGroups[i].frequencyRangeStartIndex &&
                             dataGroup.frequencyRangeEndIndex <= m_oDataGroups[i].frequencyRangeEndIndex)
                    {
						overlapDetected = true;
                    }					
					//check for an overlap with the of the start index of the indexed data group.
                    else if (m_oDataGroups[i].frequencyRangeStartIndex >= dataGroup.frequencyRangeStartIndex &&
                        	 m_oDataGroups[i].frequencyRangeStartIndex <= dataGroup.frequencyRangeEndIndex)
                    {
						overlapDetected = true; 
                    }
					//check for an overlap with the of the end index of the indexed data group.
                    else if (m_oDataGroups[i].frequencyRangeEndIndex >= dataGroup.frequencyRangeStartIndex &&
                             m_oDataGroups[i].frequencyRangeEndIndex <= dataGroup.frequencyRangeEndIndex)
                    {
						overlapDetected = true;
                    }
					
					//display warning
					if (overlapDetected)
					{						
                        Debug.LogWarning("Data Group \"" +
                                         dataGroup.dataGroupName +
                                         "\" has its frequency range overlapping with Data Group \"" +
                                         m_oDataGroups[i].dataGroupName +
                                         "\".  This is not recommended due to performance considerations.");
					}
                }
            }

        }
    }
	
	/// <summary>
	/// This adds a controller to this VisManager. 
	/// </summary>
	/// <param name="controller">
	/// This is the controller to add.
	/// </param>
	public void AddController(VisBaseController controller)
	{
		//Make sure the controller is not null, and it is not already in this VisManager.
		if (controller != null && m_oControllers.Contains(controller) == false &&
            (controller.Manager != this || GetControllerByName(controller.controllerName) == null))
        {
            //make sure there is not a controller already in here with the game object name and controller name the same
            for (int i = 0; i < m_oControllers.Count; i++)
            {
                if (m_oControllers[i].name == controller.name &&
                    m_oControllers[i].controllerName == controller.controllerName)
                {
                    Debug.Log("Ignore 1");
                    //this object may already be in there, ignore it!
                    return;
                }
            }

            //force unique name
            controller.controllerName = EnsureUniqueControllerName(controller.controllerName);

            //add the controller
            m_oControllers.Add(controller);
        }
	}	
	
	/// <summary>
	/// This removes a data group to this VisManager. 
	/// </summary>
	/// <param name="dataGroup">
	/// This is the data group to remove.
	/// </param>
	public void RemoveDataGroup(VisDataGroup dataGroup)
	{
		//Make sure the data group is not null, and it is already in this VisManager.
		if (dataGroup != null && m_oDataGroups.Contains(dataGroup))
		{
			//remove the data group
			m_oDataGroups.Remove(dataGroup);
		}
	}	
	
	/// <summary>
	/// This removes a controller to this VisManager. 
	/// </summary>
	/// <param name="controller">
	/// This is the controller to remove.
	/// </param>
	public void RemoveController(VisBaseController controller)
	{
		//Make sure the controller is not null, and it is already in this VisManager.
		if (controller != null && m_oControllers.Contains(controller))
		{
			//remove the controller
			m_oControllers.Remove(controller);
		}
	}
	
	/// <summary>
	/// This clears all data groups on this manager. 
	/// </summary>
	public void ClearDataGroups()
	{
		m_oDataGroups.Clear();	
	}
	
	/// <summary>
	/// This clears all controllers on this manager. 
	/// </summary>
	public void ClearControllers()
	{
		m_oControllers.Clear();
	}
	
	/// <summary>
	/// This gets the array of the spectrum data. 
	/// </summary>
	/// <returns>
	/// The array of the spectrum data.
	/// </returns>
	public float[] GetSpectrumData()
	{
		return m_afSpectrumData;
	}
	
	/// <summary>
	/// This gets the array of the raw audio data. 
	/// </summary>
	/// <returns>
	/// This is the array of the raw audio data.
	/// </returns>
	public float[] GetRawAudioData()
	{
		return m_afRawAudioData;
	}
	
	/// <summary>
	/// This gets a data group from this VisManager by name.
	/// </summary>
	/// <param name="dataGroupName">
	/// The name of the data group to find.
	/// </param>
	/// <returns>
	/// The data group that was requested.  Returns null if not found.
	/// </returns>
	public VisDataGroup GetDataGroupByName(string dataGroupName)
	{		
		//loop through all data groups
		for (int i = 0; i < m_oDataGroups.Count; i++)
		{
			//check if this is the data group that was requested, if so, return it.
			if (m_oDataGroups[i].dataGroupName == dataGroupName)
				return m_oDataGroups[i];
		}
		
		//requested data group not found, return null
		return null;
	}	
	
	/// <summary>
	/// This gets a controller from this VisManager by name.
	/// </summary>
    /// <param name="controllerName">
	/// The name of the controller to find.
	/// </param>
	/// <returns>
	/// The controller that was requested.  Returns null if not found.
	/// </returns>
	public VisBaseController GetControllerByName(string controllerName)
	{
		//loop through all controllers
		for (int i = 0; i < m_oControllers.Count; i++)
		{
			//check fi this is the controller that was requested, if so, return it
			if (m_oControllers[i].controllerName == controllerName)
				return m_oControllers[i];
		}
		
		//requested controller not found, return null.
		return null;
	}



    /// <summary>
    /// This ensures a controller name is unique.
    /// </summary>
    /// <param name="name">The current name to modify.</param>
    public string EnsureUniqueControllerName(string name)
    {
        //loop through and make sure that name has not been used, auto assign a unique name
        int nameTry = 1;
        string baseName = name;
        string newName = name;
        bool unique = true;
        do
        {
            //set that it is unique
            unique = true;

            //loop through all controllers and make sure the name is unique
            for (int i = 0; i < m_oControllers.Count; i++)
            {
                //the names are the same
                if (newName.Equals(m_oControllers[i].controllerName, System.StringComparison.CurrentCultureIgnoreCase))
                    unique = false;
            }

            if (!unique)
            {
                //try next name
                newName = baseName + "_" + (nameTry++).ToString();
            }
        }
        while (unique == false);

        //return the new name
        return newName;
    }

    /// <summary>
    /// This ensures a data group name is unique.
    /// </summary>
    /// <param name="name">The current name to modify.</param>
    public string EnsureUniqueDataGroupName(string name)
    {
        //loop through and make sure that name has not been used, auto assign a unique name
        int nameTry = 1;
        string baseName = name;
        string newName = name;
        bool unique = true;
        do
        {
            //set that it is unique
            unique = true;

            //loop through all data groups and make sure the name is unique
            for (int i = 0; i < m_oDataGroups.Count; i++)
            {
                //the names are the same
                if (newName.Equals(m_oDataGroups[i].dataGroupName, System.StringComparison.CurrentCultureIgnoreCase))
                    unique = false;
            }

            if (!unique)
            {
                //try next name
                newName = baseName + "_" + (nameTry++).ToString();
            }
        }
        while (unique == false);

        //return the new name
        return newName;
    }
	
	#endregion
	
	#region Debug GUI Functions
	
	/// <summary>
	/// This function displays the debug GUI for this VisManager. 
	/// </summary>
	void OnGUI()
	{
		//Check if the debug gui should be displayed, and make sure there is a debug texture assigned.
		if ((displaySpectrumDebug || displayDataGroupDebug || displayControllerDebug) && debugTexture != null)
		{			
			//get the base variables that define how to display the debug GUI
			int startX = 10;
			int startY = 20;
			int separation = debugSeparation;
			int labelHeight = 20;
			int padding = 5;
			int frameWidth = debugSpectrumBarWidth * (int)windowSize + padding * 2;
			int frameHeight = debugSpectrumBarHeight + debugRawAudioBarHeight + padding * 3 + labelHeight * 4;			
			int maxDisplayedHeight = 0;
			
			if (displaySpectrumDebug)
			{			
				//begin the gui group and draw the main label
                Rect frameRect = new Rect(startX - padding, startY - padding, frameWidth, frameHeight);
                GUI.BeginGroup(frameRect);
                GUI.color = new Color(0, 0, 0, 0.5f);
                GUI.DrawTexture(new Rect(0, 0, frameRect.width, frameRect.height), debugTexture);
                GUI.color = Color.white;
				GUI.Label(new Rect(padding, padding, 200, labelHeight + 3), "Range: " + ((m_nFrequencyRange > 0) ? (m_nFrequencyRange + " Hz") : ("N/A")));
				GUI.Label(new Rect(frameWidth - 80 - padding, padding, 80, labelHeight + 3), "Max: " + m_fMaxDebugValue.ToString("F4"));
				GUI.Label(new Rect(padding, padding + labelHeight, 200, labelHeight + 3), "Resolution: " + ((m_fFrequencyResolution > 0) ? (m_fFrequencyResolution.ToString("F1") + " Hz") : ("N/A")));
				GUI.Label(new Rect(padding, padding + labelHeight * 2, 200, labelHeight + 3), "Spectrum");
				
				//Loop through all of the spectrum data and display it.
				for (int i = 0; i < m_afSpectrumData.Length; i++) 
				{
					float value = m_afSpectrumData[i];
					if (value > m_fMaxDebugValue)
						m_fMaxDebugValue = value;
					float perc = (m_fMaxDebugValue > 0.0f) ? (value / m_fMaxDebugValue) : 0.0f;
					
					int height = (int)(((float)debugSpectrumBarHeight - 1) * perc) + 1;
	                GUI.color = GetDebugColor(i, VisDataSource.Spectrum);
					GUI.DrawTexture(new Rect(padding + debugSpectrumBarWidth * i, labelHeight * 3 + padding + debugSpectrumBarHeight - height, debugSpectrumBarWidth, height), debugTexture);
				}
				
				GUI.Label(new Rect(padding, padding + labelHeight * 3 + debugSpectrumBarHeight, 200, labelHeight + 3), "Raw Audio");
				
				//Loop through all of the raw audio data and display it.
				for (int i = 0; i < m_afRawAudioData.Length; i++) 
				{
					float value = m_afRawAudioData[i];
					float perc = Mathf.Abs(value);
					
					int height = (int)(((float)(debugRawAudioBarHeight / 2) - 1) * perc) + 1;
	                GUI.color = GetDebugColor(i, VisDataSource.Raw);
					
					if (value < 0.0f)
						GUI.DrawTexture(new Rect(padding + debugSpectrumBarWidth * i, 
						                         labelHeight * 4 + padding * 2 + debugSpectrumBarHeight + (debugRawAudioBarHeight / 2), 
						                         debugSpectrumBarWidth,
						                         height),
						                debugTexture);
					else
						GUI.DrawTexture(new Rect(padding + debugSpectrumBarWidth * i, 
						                         labelHeight * 4 + padding * 2 + debugSpectrumBarHeight + (debugRawAudioBarHeight / 2) - height + 1, 
						                         debugSpectrumBarWidth, 
						                         height), 
						                debugTexture);
				}
				
				
				//draw the frame outline and end the gui group
				GUI.color = Color.white;
				GUI.DrawTexture(new Rect(0, 0, frameWidth, 1), debugTexture);
				GUI.DrawTexture(new Rect(0, frameHeight - 1, frameWidth, 1), debugTexture);
				GUI.DrawTexture(new Rect(0, 0, 1, frameHeight), debugTexture);
				GUI.DrawTexture(new Rect(frameWidth - 1, 0, 1, frameHeight), debugTexture);
				GUI.EndGroup();		 
				
				//update start y position
				startY += frameHeight + separation;
			}
				
			//calc line wrap threshold
			int lineWrapThreshold = (int)(((float)Screen.width) * 0.7f);
			
			if (displayDataGroupDebug)
			{
				//display all data groups
				for (int i = 0; i < m_oDataGroups.Count; i++) 
				{
					//display the data group and get its displayed rectangle
					Rect displayedRect = m_oDataGroups[i].DisplayDebugGUI(startX, 
																		   startY, 
																		   debugDataGroupBarWidth, 
																		   debugDataGroupBarHeight, 
										                                   separation,
																		   debugTexture);
					
					//update the max displayed height for the gui sub frames
					if (maxDisplayedHeight < displayedRect.height)
					{
						maxDisplayedHeight = (int)displayedRect.height;
					}
					
					//update position
					startX += (int)displayedRect.width + separation;
					if (startX > lineWrapThreshold || i == m_oDataGroups.Count - 1)
					{//wrap to the next line
						startX = 10;
						startY += maxDisplayedHeight + separation;
					}
				}
			}
			
			if (displayControllerDebug)
			{
				//display all controllers
				maxDisplayedHeight = 0;
				for (int i = 0; i < m_oControllers.Count; i++) 
				{
					//display the controller and get its displayed rectangle
					Rect displayedRect = m_oControllers[i].DisplayDebugGUI(startX, 
																		   startY, 
																		   debugControllerBarWidth, 
																		   debugControllerBarHeight, 
										                                   separation,
																		   debugTexture);
					
					//update the max displayed height for the gui sub frames
					if (maxDisplayedHeight < displayedRect.height)
					{
						maxDisplayedHeight = (int)displayedRect.height;
					}
					
					//update positon
					startX += (int)displayedRect.width + separation;
					if (startX > lineWrapThreshold || i == m_oControllers.Count - 1)
					{//wrap to the next line
						startX = 10;
						startY += maxDisplayedHeight + separation;
					}
				}
			}
		}
	}
	
	/// <summary>
	/// This gets the debug gui color for a specific frequency index 
	/// </summary>
	/// <param name="freqIndex">
	/// This is the frequency index used to lookup the debug color.
	/// </param>
    /// <param name="dataSource">
    /// The data source.
    /// </param>
	/// <returns>
	/// This returns the debug color to be used to display the requested frequency index.
	/// </returns>
    private Color GetDebugColor(int freqIndex, VisDataSource dataSource)
    {
        for (int i = 0; i < m_oDataGroups.Count; i++)
        {
            if (m_oDataGroups[i] != null &&
			    m_oDataGroups[i].dataSource == dataSource &&
                freqIndex >= m_oDataGroups[i].frequencyRangeStartIndex &&
                freqIndex <= m_oDataGroups[i].frequencyRangeEndIndex)
            {
                return m_oDataGroups[i].debugColor;
            }
        }
        return Color.white;
    }
	
	#endregion
	
	#region Base Functions
	
	/// <summary>
	/// To string function. 
	/// </summary>
	/// <returns>
	/// This object as a string.
	/// </returns>
	public override string ToString ()
	{
		return "VisManager \"" + name + "\"";
	}

    #endregion

    #region Target Restore Functions

    /// <summary>
    /// This attempts to restore the last set manager on the target.
    /// </summary>
    /// <param name="target">The target to restore.</param>
    /// <returns>Whether or not the target was restored.</returns>
    public static bool RestoreVisManagerTarget(IVisManagerTarget target)
    {
        //make sure the vis manager is set, and if not, check if there 
        //is a name set and try and find that object as the manager
        if (/*target.Manager == null && */target.LastManagerName != null && target.LastManagerName.Length > 0)
        {
            //try to get that game object
            GameObject managerObject = GameObject.Find(target.LastManagerName);
            if (managerObject != null)
            {
                //try to get the manager of it
                target.Manager = managerObject.GetComponent<VisManager>();
                if (target.Manager != null)
                    return true;
            }
        }

        return false;
    }

    #endregion
}

/// <summary>
/// This interface is used by objects that need to target a VisManager. 
/// </summary>
public interface IVisManagerTarget
{
	/// <summary>
	/// This gets/sets the vis manager for this interface. 
	/// </summary>
	VisManager Manager
	{
		get;
		set;
	}

    /// <summary>
    /// This gets the name of the last manager that was set to this target.
    /// </summary>
    string LastManagerName
    {
        get;
    }
}
