using UnityEngine;

#region Enumerations

/// <summary>
/// This enumeration describes what value from the target controller 
/// should be used as the source value to modify a property. 
/// </summary>
public enum ControllerSourceValue
{
	/// <summary>
	/// This indicates the current value of the target controller. 
	/// </summary>
	Current,
	
	/// <summary>
	/// This indicates the previous value of the target controller. 
	/// </summary>
	Previous,
	
	/// <summary>
	/// This indicates the difference value of the target controller. 
	/// </summary>
	Difference
}

#endregion

/// <summary>
/// This static class is used to provide helper functions for Visualizer Studio. 
/// </summary>
public static class VisHelper
{		
	#region Validation Functions
	
	/// <summary>
	/// This functon attempts to validiate a value. 
	/// </summary>
	/// <param name="currentValue">
	/// The current value to validate.
	/// </param>
	/// <param name="minValue">
	/// The min valid value.
	/// </param>
	/// <param name="maxValue">
	/// The max valid value.
	/// </param>
	/// <param name="defaultValue">
	/// The default value to use if the current value is invalid.
	/// </param>
	/// <param name="obj">
	/// The object whose data is being validated.
	/// </param>
	/// <param name="fieldName">
	/// The name of the field being validated.
	/// </param>
	/// <param name="error">
	/// Whether or not this is an error or warning if invalid.
	/// </param>
	/// <returns>
	/// The new, validated value.
	/// </returns>
	public static int Validate(int currentValue, int minValue, int maxValue, int defaultValue, Object obj, string fieldName, bool error)
	{
		//check if the current value is out of the valid range
		if (currentValue < minValue || currentValue > maxValue)
		{
			//construct the log message
			string message = obj.ToString() + 
							 " has its data \"" + 
							 fieldName + 
							 "\" out of its valid data range of " + 
							 minValue + 
							 " to " + 
							 maxValue +  
							 ".  Defaulting to " + 
							 defaultValue +  
							 ".";
			
			//display the log message
			if (error)
				Debug.LogError(message, obj);
			else
				Debug.LogWarning(message, obj);
			
			//current value is invalid, return default value
			return defaultValue;
		}
		
		//current value is valid, return current value
		return currentValue;
	}
	
	/// <summary>
	/// This functon attempts to validiate a value. 
	/// </summary>
	/// <param name="currentValue">
	/// The current value to validate.
	/// </param>
	/// <param name="minValue">
	/// The min valid value.
	/// </param>
	/// <param name="maxValue">
	/// The max valid value.
	/// </param>
	/// <param name="defaultValue">
	/// The default value to use if the current value is invalid.
	/// </param>
	/// <param name="obj">
	/// The object whose data is being validated.
	/// </param>
	/// <param name="fieldName">
	/// The name of the field being validated.
	/// </param>
	/// <param name="error">
	/// Whether or not this is an error or warning if invalid.
	/// </param>
	/// <returns>
	/// The new, validated value.
	/// </returns>
	public static float Validate(float currentValue, float minValue, float maxValue, float defaultValue, Object obj, string fieldName, bool error)
	{
		//check if the current value is out of the valid range
		if (currentValue < minValue || currentValue > maxValue)
		{
			//construct the log message
			string message = obj.ToString() + 
							 " has its data \"" + 
							 fieldName + 
							 "\" out of its valid data range of " + 
							 minValue + 
							 " to " + 
							 maxValue +  
							 ".  Defaulting to " + 
							 defaultValue +  
							 ".";
			
			//display the log message
			if (error)
				Debug.LogError(message, obj);
			else
				Debug.LogWarning(message, obj);
			
			//current value is invalid, return default value
			return defaultValue;
		}
		
		//current value is valid, return current value
		return currentValue;
	}
	
	#endregion

    #region Range Functions

    /// <summary>
    /// This function converts between a source range and a destination range based on the current source value.
    /// So given the following source and dest values...
    /// sourceValue=1.25, sourceMin=1.0, sourceMax=2.0, destMin=4.0, destMax=8.0
    /// then...
    /// convertedValue=5.0
    /// </summary>
    /// <param name="sourceValue">This is the current source value to convert from.</param>
    /// <param name="sourceMin">This is the minimum value for the source range to convert from.</param>
    /// <param name="sourceMax">This is the maximum value for the source range to convert from.</param>
    /// <param name="destMin">This is the minimum value for the dest range to convert to.</param>
    /// <param name="destMax">This is the maximum value for the dest range to convert to.</param>
    /// <param name="invertDestValue">Indicates whether or not to invert to dest range result.</param>
    /// <returns>The converted value.</returns>
    public static float ConvertBetweenRanges(float sourceValue, float sourceMin, float sourceMax, float destMin, float destMax, bool invertDestValue)
    {
        //calc the percentage
        float percentage = 0.0f;
        if (sourceValue <= sourceMin)
            percentage = 0.0f;
        else if (sourceValue >= sourceMax)
            percentage = 1.0f;
        else
            percentage = (sourceValue - sourceMin) / (sourceMax - sourceMin);

        //invert the percentage if needed
        if (invertDestValue)
            percentage = 1.0f - percentage;

        //return the new value
        return Mathf.Lerp(destMin, destMax, percentage);
    }

    #endregion
}

