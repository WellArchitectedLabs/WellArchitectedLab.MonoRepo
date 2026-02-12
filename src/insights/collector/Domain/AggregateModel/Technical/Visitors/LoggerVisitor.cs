using Microsoft.Extensions.Logging;

namespace WeatherInsights.Collector.Domain.AggregateModel.Technical.Visitors;

/// <summary>
/// Extensions around <see cref="ILogger"/>
/// </summary>
public static class loggerExtensions
{
    /// <summary>
    /// Logs a validation result list via the provided logger object
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/>to extend</param>
    /// <param name="validationResults">validation results containing mixed errors / warnings and informational validation messages</param>
    public static void LogValidationResults(this ILogger logger, List<ValidationResult> validationResults)
    {
        foreach (var validationResult in validationResults)
        {
            switch (validationResult)
            {
                case { HasErrors: true }:
                    logger.LogError(validationResult.ValidationMessage);
                    break;

                case { HasWarnings: true }:
                    logger.LogWarning(validationResult.ValidationMessage);
                    break;

                case { IsSuccessful: true }:
                    logger.LogInformation(validationResult.ValidationMessage);
                    break;
            }
        }
    }
}