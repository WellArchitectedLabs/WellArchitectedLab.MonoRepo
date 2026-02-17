namespace WeatherInsights.Collector.Domain.AggregateModel.Technical.Enums;

/// <summary>
/// Status enum, used in validation result
/// </summary>
public enum ValidationStatus
{
    /// <summary>
    /// Validation is Ok
    /// </summary>
    Success,

    /// <summary>
    /// Validatable object is not valid
    /// </summary>
    Error,

    /// <summary>
    /// Data has validation warnings
    /// </summary>
    Warning
}