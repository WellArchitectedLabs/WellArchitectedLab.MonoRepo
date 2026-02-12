using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using Microsoft.Extensions.Logging;
using WeatherInsights.Collector.Domain.AggregateModel.Technical;
using WeatherInsights.Collector.Domain.AggregateModel.Technical.Enums;
using WeatherInsights.Collector.Domain.Ports.Config;

namespace WeatherInsights.Collector.Application.Extensions;

/// <summary>
/// Utility methods around <see cref="WfActualDto"/>
/// </summary>
public static class WfActualExtensions
{
    /// <summary>
    /// Gets actual timestamp in <see cref="DateOnly"/> format
    /// </summary>
    /// <param name="wfActualDto"></param>
    public static DateOnly GetDateOnlyTimeStamp(this WfActualDto wfActualDto) =>
        DateOnly.FromDateTime(wfActualDto.TimestampUtc);
    
    /// <summary>
    /// Validates the provided list of wf actuals
    /// </summary>
    /// <param name="wfActuals">list of wf actuals to validate</param>
    /// <param name="referenceDate">reference date</param>
    /// <param name="masterDataApiParameters"><see cref="PredictionEngineConfig"/>: necessary to validate timestamp near / yearly partitioning.</param>
    /// <returns></returns>
    public static List<ValidationResult> Validate(
        this IEnumerable<WfActualDto> wfActuals,
        DateOnly referenceDate,
        MasterDataApiParameters masterDataApiParameters)
    {   
        var actualTimestamps = wfActuals
            .Select(x => x.TimestampUtc)
            .ToHashSet();

        // Reference window end: referenceDate at 23:00 UTC
        var referenceEndUtc = referenceDate
            .ToDateTime(new TimeOnly(23, 00, 00),
                DateTimeKind.Utc);

        var validationResults = ValidateWindow(
            windowName: "Near history rolling window",
            referenceEndUtc,
            masterDataApiParameters.RollingWindowDays,
            actualTimestamps);

        for (int yearOffset = 1; yearOffset <= masterDataApiParameters.YearsHistoryDepth; yearOffset++)
        {
            var yearlyReferenceEndUtc = referenceEndUtc.AddYears(-yearOffset);

            validationResults.AddRange(ValidateWindow(
                windowName: $"Year -{yearOffset} rolling window history",
                yearlyReferenceEndUtc,
                masterDataApiParameters.RollingWindowDays,
                actualTimestamps));
        }

        return validationResults;
    }
    
    /// <summary>
    /// Validates a window of timestamps
    /// </summary>
    /// <param name="windowName"></param>
    /// <param name="referenceEndUtc"></param>
    /// <param name="rollingWindowDays"></param>
    /// <param name="actualTimestamps"></param>
    /// <returns></returns>
    private static List<ValidationResult> ValidateWindow(
        string windowName,
        DateTime referenceEndUtc,
        int rollingWindowDays,
        HashSet<DateTime> actualTimestamps)
    {
        var expectedTimestamps = BuildExpectedHourlyTimestamps(
            referenceEndUtc,
            rollingWindowDays);

        var missing = expectedTimestamps.Except(actualTimestamps).ToList();
        var extra = actualTimestamps.Except(expectedTimestamps).ToList();
        var results = new List<ValidationResult>();

        if (missing.Any())
        {
            results.Add(new ValidationResult(
                "Actuals Hours Validation",
                ValidationStatus.Warning,
                $"{windowName}: Missing {missing.Count} expected hourly timestamps."));
        }

        if (extra.Any())
        {
            results.Add(new ValidationResult(
                "Actuals Hours Validation",
                ValidationStatus.Warning,
                $"{windowName}: Found {extra.Count} unexpected timestamps."));
        }

        if (!missing.Any() && !extra.Any())
        {
            results.Add(new ValidationResult(
                "Actuals Hours Validation",
                ValidationStatus.Success,
                $"{windowName}: Cardinality and timestamps are valid."));
        }

        return results;
    }
    
    /// <summary>
    /// Generates the expected timestamps to validate against
    /// </summary>
    /// <param name="referenceEndUtc"></param>
    /// <param name="rollingWindowDays"></param>
    /// <returns></returns>
    private static HashSet<DateTime> BuildExpectedHourlyTimestamps(
        DateTime referenceEndUtc,
        int rollingWindowDays)
    {
        var totalHours = rollingWindowDays * 24;
        var startUtc = referenceEndUtc.AddHours(-(totalHours - 1));

        var expected = new HashSet<DateTime>();

        for (int i = 0; i < totalHours; i++)
        {
            var ts = startUtc.AddHours(i);

            // Enforce exact hour alignment
            expected.Add(new DateTime(
                ts.Year,
                ts.Month,
                ts.Day,
                ts.Hour,
                0,
                0,
                DateTimeKind.Utc));
        }

        return expected;
    }
}