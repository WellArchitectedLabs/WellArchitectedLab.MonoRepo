using WeatherInsights.Collector.Domain.AggregateModel.Audit.Enums;

namespace WeatherInsights.Collector.Domain.AggregateModel.Audit;

/// <summary>
/// Weather insights need to be reproducible.
/// This entity stored the input / output payloads for engine calls,
/// stored by execution time
/// </summary>
public class WfInsightAudit
{
    /// <summary>
    /// The time calculation was required from prediction engine in UTC
    /// </summary>
    public required DateTime RequestTimeUtc { get; init; }
    
    /// <summary>
    /// The time calculation was received from prediction engine in UTC
    /// </summary>
    public required  DateTime ResponseTimeUtc { get; init; }
    
    /// <summary>
    /// The input for weather forecasting engine for data repro.
    /// </summary>
    public required string WeatherEngineInput { get; init; }
    
    /// <summary>
    /// The input from weather forecasting engine for data repro.
    /// </summary>
    public required string WeatherEngineOutput { get; init; }
    
    /// <summary>
    /// The associated weather insight
    /// Is null in case the execution finished with and no insight is stored
    /// </summary>
    public int? WeatherInsightId { get; init; }
}