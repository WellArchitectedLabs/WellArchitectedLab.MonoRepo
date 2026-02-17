using Microsoft.Extensions.Logging;

namespace WeatherInsights.Collector.Domain.AggregateModel.Technical.Extensions;

/// <summary>
/// Extensions around <see cref="ILogger"/>
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Sanitizes a message before it is written to logs, to prevent log forging.
    /// Currently removes carriage return and newline characters.
    /// </summary>
    private static string SanitizeForLogging(string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return string.Empty;
        }

        return message
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);
    }

    /// <summary>
    /// Logs a validation result list via the provided logger object
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/>to extend</param>
    /// <param name="validationResults">validation results containing mixed errors / warnings and informational validation messages</param>
    public static void LogValidationResults(this ILogger logger, List<ValidationResult> validationResults)
    {
        foreach (var validationResult in validationResults)
        {
            var sanitizedMessage = SanitizeForLogging(validationResult.ValidationMessage);

            switch (validationResult)
            {
                case { HasErrors: true }:
                    logger.LogError(sanitizedMessage);
                    break;

                case { HasWarnings: true }:
                    logger.LogWarning(sanitizedMessage);
                    break;

                case { IsSuccessful: true }:
                    logger.LogInformation(sanitizedMessage);
                    break;
            }
        }
    }
}