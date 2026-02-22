using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using WeatherInsights.Collector.Domain.AggregateModel.Technical;
using WeatherInsights.Collector.Domain.AggregateModel.Technical.Enums;
using WeatherInsights.Collector.Domain.Ports.Config;

namespace WeatherInsights.Collector.Domain.AggregateModel.Insight.Extensions;

/// <summary>
/// Utility methods around <see cref="GetHistoricalSlicesDto"/>
/// </summary>
public static class GetHistoricalSlicesDtoExtensions
{
    /// <summary>
    /// Gets actual timestamp in <see cref="DateOnly"/> format
    /// </summary>
    /// <param name="wfActualDto"></param>
    public static DateOnly GetDateOnlyTimeStamp(this GetHistoricalSlicesDto wfActualDto) =>
        DateOnly.FromDateTime(wfActualDto.TimestampUtc);
    
    /// <summary>
    /// Validates the provided list of wf actuals
    /// </summary>
    /// <param name="wfActuals">list of wf actuals to validate</param>
    /// <param name="referenceDate">reference date</param>
    /// <param name="masterDataApiParameters"><see cref="MasterDataApiParameters"/>: necessary to validate timestamp near / yearly partitioning.</param>
    /// <param name="thresholds">Application settings threshold</param>
    /// <returns></returns>
    public static List<ValidationResult> Validate(
        this IEnumerable<GetHistoricalSlicesDto> wfActuals,
        DateOnly referenceDate,
        MasterDataApiParameters masterDataApiParameters,
        TimeStampsThresholdsConfig thresholds)
    {
        var actualTimestamps = wfActuals
            .Select(x => x.TimestampUtc)
            .ToHashSet();
        // Reference window end: referenceDate at 23:00 UTC
        var referenceEndUtc = referenceDate
            .ToDateTime(new TimeOnly(23, 0, 0), DateTimeKind.Utc);

        var validationResults = new List<ValidationResult>();

        validationResults.AddRange(ValidateWindow(
            windowName: "Near history rolling window",
            referenceEndUtc,
            masterDataApiParameters.RollingWindowDays,
            actualTimestamps,
            thresholds));

        for (int yearOffset = 1; yearOffset <= masterDataApiParameters.YearsHistoryDepth; yearOffset++)
        {
            var yearlyReferenceEndUtc = referenceEndUtc.AddYears(-yearOffset);

            validationResults.AddRange(ValidateWindow(
                windowName: $"Year -{yearOffset} rolling window history",
                yearlyReferenceEndUtc,
                masterDataApiParameters.RollingWindowDays,
                actualTimestamps,
                thresholds));
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
    /// <param name="thresholds"></param>
    /// <returns></returns>
    private static List<ValidationResult> ValidateWindow(
        string windowName,
        DateTime referenceEndUtc,
        int rollingWindowDays,
        HashSet<DateTime> actualTimestamps,
        TimeStampsThresholdsConfig thresholds)
    {
        var expectedTimestamps = BuildExpectedHourlyTimestamps(
            referenceEndUtc,
            rollingWindowDays);

        var missing = expectedTimestamps.Except(actualTimestamps).ToList();
        var extra = actualTimestamps.Except(expectedTimestamps).ToList();

        var expectedCount = expectedTimestamps.Count;
        var results = new List<ValidationResult>();

        // ---------- Missing timestamps ----------
        if (missing.Any())
        {
            var missingPercentage =
                (missing.Count * 100d) / expectedCount;

            var status = missingPercentage >
                         thresholds.MaxToleratedMissingHoursPercentage
                ? ValidationStatus.Error
                : ValidationStatus.Warning;

            results.Add(new ValidationResult(
                "ActualsMissingTimestamps",
                status,
                $"{windowName}: Missing {missing.Count}/{expectedCount} timestamps " +
                $"({missingPercentage:F2}%). " +
                $"Threshold: {thresholds.MaxToleratedMissingHoursPercentage}%."
            ));
        }

        // ---------- Extra timestamps ----------
        if (extra.Any())
        {
            var status = thresholds.EnforceErrorOnUnrelatedTimeStamps
                ? ValidationStatus.Error
                : ValidationStatus.Warning;

            results.Add(new ValidationResult(
                "ActualsExtraTimestamps",
                status,
                $"{windowName}: Found {extra.Count} unexpected timestamps."
            ));
        }

        // ---------- Success ----------
        if (!missing.Any() && !extra.Any())
        {
            results.Add(new ValidationResult(
                "ActualsHoursValidation",
                ValidationStatus.Success,
                $"{windowName}: Cardinality and timestamps are valid."
            ));
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