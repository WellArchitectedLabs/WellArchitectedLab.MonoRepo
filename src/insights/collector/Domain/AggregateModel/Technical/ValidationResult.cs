using WeatherInsights.Collector.Domain.AggregateModel.Technical.Enums;

namespace WeatherInsights.Collector.Domain.AggregateModel.Technical;

/// <summary>
/// Validation result structure.
/// Used as a value object as a result of data validation in wf insights service
/// </summary>
/// <param name="ValidationStatus"></param>
/// <param name="UnsuccessfulValidationMessage"></param>
public record ValidationResult(
    string Context,
    ValidationStatus ValidationStatus,
    string? UnsuccessfulValidationMessage = null)
{
    public bool IsSuccessful =>  ValidationStatus == ValidationStatus.Success;
    public bool HasErrors =>  ValidationStatus == ValidationStatus.Error;
    public bool HasWarnings =>  ValidationStatus == ValidationStatus.Warning;
}