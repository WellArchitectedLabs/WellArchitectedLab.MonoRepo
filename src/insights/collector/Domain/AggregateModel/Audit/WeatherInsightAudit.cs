using WeatherInsights.Collector.Domain.AggregateModel.Audit.Enums;

namespace WeatherInsights.Collector.Domain.AggregateModel.Audit;

/// <summary>
/// Weather insights need to be reproducible.
/// This entity stored the input / output payloads for engine calls,
/// stored by execution time
/// </summary>
public class WeatherInsightAudit
{
    /// <summary>
    /// The time calculation was executed in UTC
    /// </summary>
    public DateTime ExecutionTimeUtc { get; init; }
    /// <summary>
    /// The concerned timestamp in UTC
    /// </summary>
    public DateTime TimestampUtc { get; init; }
    /// <summary>
    /// The input for weather forecasting engine for data repro.
    /// </summary>
    public required string WeatherEngineInput { get; init; }
    /// <summary>
    /// The input from weather forecasting engine for data repro.
    /// </summary>
    public required string WeatherEngineOutput { get; init; }
    /// <summary>
    /// Execution status
    /// </summary>
    public required WeatherEngineExecutionStatus WeatherEngineExecutionStatus { get; init; }
    /// <summary>
    /// The associated weather insight
    /// Is null in case the execution finished with and no insight is stored
    /// </summary>
    public int? WeatherInsightId { get; init; }
}