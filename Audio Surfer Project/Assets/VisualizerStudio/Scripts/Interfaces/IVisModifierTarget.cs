using System;

/// <summary>
/// This interface should be used by any component that wants to be a target of 
/// a modifier.  When set as a target of a modifier, OnValueUpdated will be 
/// called every time the value is updated. 
/// </summary>
public interface IVisModifierTarget
{	
	/// <summary>
	/// This function is called by the base modifier whenever 
	/// the value of the targeted controller is updated.
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
	void OnValueUpdated(float current, float previous, float difference, float adjustedDifference);
}

