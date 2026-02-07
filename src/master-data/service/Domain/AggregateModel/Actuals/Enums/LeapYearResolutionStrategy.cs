namespace MasterData.Domain.AggregateModel.Actuals.Enums;

/// <summary>
/// Leap day is identified as the 29th of February for leap years
/// </summary>
public enum LeapDayResolutionStrategy
{
    /// <summary>
    /// If a leap day is identified, fallback to the 28th (Day - 1) of Feb for historical depth resolution
    /// </summary>
    ClampToFeb28,
    /// <summary>
    /// If a leap day is identified, fallback to the 01rst of Mars (Day + 1) for historical depth resolution
    /// </summary>
    ShiftToMar01
}