using System;
using UnityEngine;

/// <summary>
/// This class defines a data container for visualization data. 
/// </summary>
public class VisDataContainer
{		
	#region Public Member Variables
	
	/// <summary>
	/// This indicates the average value. 
	/// </summary>
	public float average = 0.0f;
	
	/// <summary>
	/// This indicates the previous average value. 
	/// </summary>
	public float previousAverage = 0.0f;
	
	/// <summary>
	/// This indicates the difference of the average value. 
	/// </summary>
	public float averageDifference = 0.0f;
	
	/// <summary>
	/// This indicates the median value. 
	/// </summary>
	public float median = 0.0f;
	
	/// <summary>
	/// This indicates the previous median value. 
	/// </summary>
	public float previousMedian = 0.0f;
	
	/// <summary>
	/// This indicates the difference of the median value. 
	/// </summary>
	public float medianDifference = 0.0f;
	
	/// <summary>
	/// This indicates the sum value. 
	/// </summary>
	public float sum = 0.0f;
	
	/// <summary>
	/// This indicates the previous sum value. 
	/// </summary>
	public float previousSum = 0.0f;
	
	/// <summary>
	/// This indicates the difference of the sum value. 
	/// </summary>
	public float sumDifference = 0.0f;
	
	/// <summary>
	/// This indicates the minimum value. 
	/// </summary>
	public float minimum = 0.0f;
	
	/// <summary>
	/// This indicates the previous minimum value. 
	/// </summary>
	public float previousMinimum = 0.0f;
	
	/// <summary>
	/// This indicates the difference of the minimum value. 
	/// </summary>
	public float minimumDifference = 0.0f;
	
	/// <summary>
	/// This indicates the maximum value. 
	/// </summary>
	public float maximum = 0.0f;
	
	/// <summary>
	/// This indicates the previous maximum value. 
	/// </summary>
	public float previousMaximum = 0.0f;
	
	/// <summary>
	/// This indicates the difference of the maximum value. 
	/// </summary>
	public float maximumDifference = 0.0f;
	
	#endregion
	
	#region Accessor Functions
	
	/// <summary>
	/// This gets the value based on type. 
	/// </summary>
	/// <param name="valueType">
	/// This type of value to get.
	/// </param>
	/// <returns>
	/// The value that was requested.
	/// </returns>
	public float GetValue(VisDataValueType valueType)
	{
		switch (valueType)
		{
			default:
			case VisDataValueType.Average:
				return average;
			case VisDataValueType.Median:
				return median;
			case VisDataValueType.Sum:
				return sum;
			case VisDataValueType.Minimum:
				return minimum;
			case VisDataValueType.Maximum:
				return maximum;
		}
	}
		
	/// <summary>
	/// This gets the previous value based on type. 
	/// </summary>
	/// <param name="valueType">
	/// This type of previous value to get.
	/// </param>
	/// <returns>
	/// The previous value that was requested.
	/// </returns>
	public float GetPreviousValue(VisDataValueType valueType)
	{
		switch (valueType)
		{
			default:
			case VisDataValueType.Average:
				return previousAverage;
			case VisDataValueType.Median:
				return previousMedian;
			case VisDataValueType.Sum:
				return previousSum;
			case VisDataValueType.Minimum:
				return previousMinimum;
			case VisDataValueType.Maximum:
				return previousMaximum;
		}
	}
		
	/// <summary>
	/// This gets the value difference based on type. 
	/// </summary>
	/// <param name="valueType">
	/// This type of value difference to get.
	/// </param>
	/// <returns>
	/// The value difference that was requested.
	/// </returns>
	public float GetValueDifference(VisDataValueType valueType)
	{
		switch (valueType)
		{
			default:
			case VisDataValueType.Average:
				return averageDifference;
			case VisDataValueType.Median:
				return medianDifference;
			case VisDataValueType.Sum:
				return sumDifference;
			case VisDataValueType.Minimum:
				return minimumDifference;
			case VisDataValueType.Maximum:
				return maximumDifference;
		}
	}
	
	#endregion
	
	#region Helper Functions
	
	/// <summary>
	/// This updates the previous value of this data container by setting the current value to the previous. 
	/// </summary>
	public void UpdatePreviousValues()
	{			
		//update previous values
		previousAverage = average;
		previousMedian = median;
		previousSum = sum;
		previousMinimum = minimum;
		previousMaximum = maximum;
	}
	
	/// <summary>
	/// This resets all current values back to their default values. 
	/// </summary>
	public void ResetCurrentValues()
	{			
		//reset current values
		average = 0.0f;
		median = 0.0f;
		sum = 0.0f;
		minimum = 10000.0f;
		maximum = -10000.0f;
	}
	
	/// <summary>
	/// This applies a boost and cutoff to all current values. 
	/// </summary>
	/// <param name="boost">
	/// The boost to apply.
	/// </param>
	/// <param name="cutoff">
	/// The cutoff to apply.
	/// </param>
	public void ApplyBoostAndCutoff(float boost, float cutoff)
	{
		average = Mathf.Clamp(average * boost, average < 0.0f ? -cutoff : 0.0f, average < 0.0f ? 0.0f : cutoff);
		median = Mathf.Clamp(median * boost, median < 0.0f ? -cutoff : 0.0f, median < 0.0f ? 0.0f : cutoff);
		sum = Mathf.Clamp(sum * boost, boost < 0.0f ? -cutoff : 0.0f, boost < 0.0f ? 0.0f : cutoff);
		minimum = Mathf.Clamp(minimum * boost, minimum < 0.0f ? -cutoff : 0.0f, minimum < 0.0f ? 0.0f : cutoff);
		maximum = Mathf.Clamp(maximum * boost, maximum < 0.0f ? -cutoff : 0.0f, maximum < 0.0f ? 0.0f : cutoff);
	}
	
	/// <summary>
	/// This updates all value differences by subtracting the current by the previous of each value type. 
	/// </summary>
	public void UpdateValueDifferences()
	{		
		//update differences
		averageDifference = average - previousAverage;
		medianDifference = median - previousMedian;
		sumDifference = sum - previousSum;
		minimumDifference = minimum - previousMinimum;
		maximumDifference = maximum - previousMaximum;
	}
	
	#endregion
}


