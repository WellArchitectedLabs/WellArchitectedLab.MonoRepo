namespace WeatherInsights.Collector.Domain.AggregateModel.Audit.Enums;

/// <summary>
/// Weather engine execution status
/// </summary>
public enum WeatherEngineExecutionStatus
{
    /// <summary>
    /// Weather engine successfully executed and returned a valid results
    /// </summary>
    Ok,
    /// <summary>
    /// Weather engine finished with errors
    /// </summary>
    Ko,
    /// <summary>
    /// Weather engine returned results, but some warnings are reported.
    /// </summary>
    Degraded
}