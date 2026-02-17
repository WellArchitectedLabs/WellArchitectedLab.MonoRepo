namespace WeatherInsights.Collector.Domain.Ports.HttpClients.Models;

/// <summary>
/// The output returned from prediction engine
/// Validation logic is external. Please check Validations folder.
/// </summary>
public record WfEngineOutput
{
    /// <summary>
    /// City id concerned by calculation
    /// </summary>
    public required int CityId { get; init; }
    /// <summary>
    /// Reference date concerned by calculation
    /// </summary>
    public required DateTime ReferenceDate { get; init; }
    
    /// <summary>
    /// Engine responded on this date time
    /// </summary>
    public DateTime ResponseTime = DateTime.UtcNow;
    
    /// <summary>
    /// A prediction indexed by hour of the reference date
    /// </summary>
    public required IDictionary<DateTime, WfEngineInsightOutputItem> PerHourPrediction { get; init; }
}

/// <summary>
/// Output for a single timeStamp (date + exact hour)
/// </summary>
public record WfEngineInsightOutputItem
{
    /// <summary>
    /// Predicted temperature in Celsius
    /// </summary>
    public required decimal Temperature { get; init; }
    /// <summary>
    /// Predicted wind speed
    /// </summary>
    public required decimal WindSpeed { get; init; }
    /// <summary>
    /// Predicted Precipitation
    /// </summary>
    public required decimal Precipitation { get; init; }
}