namespace MasterData.Client.Dtos.Parameters;

/// <summary>
/// Compact version of API history params
/// Please fill this object with search params.
/// This object will be converted internally by the connector to a query style param, interpretable by the downstream API.
/// </summary>
/// <param name="HistoricalDepthYears">number of past years of history returned for prediction analysis</param>
/// <param name="RollingWindowDays">odd number representing the number of days to pick in every historical year.</param>
/// <param name="LeapDayResolutionStrategy">resolving strategy for leap years (29th of February)</param>
public record HistorySearchParams(
    int HistoricalDepthYears, 
    int RollingWindowDays, 
    LeapDayResolutionStrategy LeapDayResolutionStrategy);


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