using System;

/// <summary>
/// This interface should be used by any component that wants to be a target of 
/// a trigger.  When set as a target of a trigger, OnTriggered will be 
/// called every time the trigger is triggered. 
/// </summary>
public interface IVisTriggerTarget
{
	/// <summary>
	/// This function is called by the base trigger whenever 
	/// this trigger has been TRIGGERED.
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
	void OnTriggered(float current, float previous, float difference, float adjustedDifference);
}


