using WeatherInsights.Collector.Domain.AggregateModel.Technical.Enums;

namespace WeatherInsights.Collector.Domain.AggregateModel.Technical;

/// <summary>
/// Validation result structure.
/// Used as a value object as a result of data validation in wf insights service
/// </summary>
/// <param name="Context">Subdomain of validation</param>
/// <param name="ValidationStatus">Status reflects the validation type</param>
/// <param name="ValidationMessage">The success / error / warning message</param>
public record ValidationResult(
    string Context,
    ValidationStatus ValidationStatus,
    string? ValidationMessage)
{
    public bool IsSuccessful =>  ValidationStatus == ValidationStatus.Success;
    public bool HasErrors =>  ValidationStatus == ValidationStatus.Error;
    public bool HasWarnings =>  ValidationStatus == ValidationStatus.Warning;
    
    /// <summary>
    /// Converts a validation result to a string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {   
        return "Context: " + Context + 
                   ", ValidationStatus: " + ValidationStatus + 
                   ", ValidationMessage: " + ValidationMessage;
    }
}