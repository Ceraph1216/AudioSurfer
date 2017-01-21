using System;
using UnityEngine;

/// <summary>
/// This class is a sub data group that belongs to a parent data group. 
/// It is used to help aggregate data for a data group.  
/// </summary>
public sealed class VisSubDataGroup : VisDataContainer
{		
	#region Public Member Variables
	
	/// <summary>
	/// This is the start frequency index of this sub data group. 
	/// </summary>
	public int frequencyRangeStartIndex = 0;
	
	/// <summary>
	/// This is the end frequency index of this sub data group. 
	/// </summary>
	public int frequencyRangeEndIndex = 1;
	
	#endregion
	
	#region Init Functions
	
	/// <summary>
	/// Constructor. 
	/// </summary>
	/// <param name="startIndex">
	/// The start frequency index.
	/// </param>
	/// <param name="endIndex">
	/// This end frequency index.
	/// </param>
	public VisSubDataGroup(int startIndex, int endIndex)
	{
		frequencyRangeStartIndex = startIndex;
		frequencyRangeEndIndex = endIndex;
	}
	
	#endregion
	
	#region Update Functions
	
	/// <summary>
	/// This is the main update function of this sub data group, uses to aggregate the data for this sub data group. 
	/// </summary>
	/// <param name="spectrum">
	/// This is the current spectrum data to update against.
	/// </param>
	public void Update(float[] spectrum)
	{
		//make sure the spectrum array was valid
		if (spectrum != null)
		{
			//update base values
			UpdatePreviousValues();
			ResetCurrentValues();
			
			//update and calculate current values			
			int countedIndices = 0;
			for (int i = frequencyRangeStartIndex; i <= frequencyRangeEndIndex && i < spectrum.Length; i++)
			{
				//increment the number of indices counted
				countedIndices++;
				
				//get the value
				float value = spectrum[i];
				
				//add to sum
				sum += value;
				
				//update min/max
				if (Mathf.Abs(value) < minimum)
					minimum = value;
				if (Mathf.Abs(value) > maximum)
					maximum = value;
			}
			
			//make sure there were counted indices
			if (countedIndices > 0)
			{
				//calc average and median
				average = sum / (float)(countedIndices);
				median = (minimum + maximum) * 0.5f;
			}
			else
			{
				//no indices counted, reset to zero
				average = 0.0f;
				median = 0.0f;
				sum = 0.0f;
				maximum = 0.0f;
				minimum = 0.0f;
			}
			
			//update differences
			UpdateValueDifferences();
		}
	}
	
	#endregion
}

