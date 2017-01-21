using System;

/// <summary>
/// This interface is used to notify prefabs/objects that are spawned by the
/// VisSpawnPrefabTrigger object, and to feed into them the current controller
/// state.
/// </summary>
public interface IVisPrefabSpawnedTarget
{
    /// <summary>
    /// This function is called by VisSpawnPrefabTrigger when it spawns a prefab.
    /// </summary>
    /// <param name="current">
    /// The current value of the associated controller.
    /// </param>
    /// <param name="previous">
    /// The previous value of the associated controller.
    /// </param>
    /// <param name="difference">
    /// The value difference of the associated controller.
    /// </param>
    /// <param name="adjustedDifference">
    /// The adjusted value difference of the targeted controller.
    /// This value is the difference value as if it took place over a 
    /// certain time period, controlled by VisBaseController.mc_fTargetAdjustedDifferenceTime.  The 
    /// default of this essientially indicates a frame rate of 60 fps to determine 
    /// the adjusted difference.  This should be used for almost all difference 
    /// calculations, as it is NOT frame rate dependent.
    /// </param>
    void OnSpawned(float current, float previous, float difference, float adjustedDifference);
}


