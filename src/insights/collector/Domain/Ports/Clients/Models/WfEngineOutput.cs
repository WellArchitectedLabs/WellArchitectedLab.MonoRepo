namespace WeatherInsights.Collector.Domain.Ports.Clients.Models;

/// <summary>
/// The output returned from prediction engine
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
    public required DateOnly ReferenceDate { get; init; }
    
    /// <summary>
    /// A prediction indexed by hour of the reference date
    /// </summary>
    public required IDictionary<DateTime, WfEngineOutput> PerHourPrediction { get; init; }
}

/// <summary>
/// Output for a single timeStamp (date + exact hour)
/// </summary>
public record WfEngineInsightOutput
{
    /// <summary>
    /// Predicted temperature in Celsius
    /// </summary>
    public required double Temperature { get; init; }
    /// <summary>
    /// Predicted wind speed
    /// </summary>
    public required double WindSpeed { get; init; }
    /// <summary>
    /// Predicted Precipitation
    /// </summary>
    public required double Precipitation { get; init; }
}