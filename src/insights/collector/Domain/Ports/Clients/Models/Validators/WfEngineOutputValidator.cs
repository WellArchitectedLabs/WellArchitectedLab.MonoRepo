using WeatherInsights.Collector.Domain.AggregateModel.Technical;
using WeatherInsights.Collector.Domain.AggregateModel.Technical.Enums;
using WeatherInsights.Collector.Domain.Ports.Config;

namespace WeatherInsights.Collector.Domain.Ports.Clients.Models.Validators;

public static class WfEngineOutputValidator
{
    private const int ExpectedHoursPerDay = 24;

    public static IReadOnlyCollection<ValidationResult> Validate(
        this WfEngineOutput wfEngineOutput,
        DateOnly referenceDate,
        TimeStampsThresholdsConfig thresholds)
    {
        var reportedDateTimes = wfEngineOutput.PerHourPrediction.Keys.ToList();
        var reportedHours = reportedDateTimes
            .Where(d => DateOnly.FromDateTime(d) == referenceDate)
            .Select(d => d.Hour)
            .Distinct()
            .Distinct()
            .ToList();
        
        var validations = GenerateMissingHoursValidation(referenceDate, reportedHours, thresholds);
        validations.AddRange(GenerateUnrelatedHoursValidation(referenceDate, reportedDateTimes, thresholds));

        return validations;
    }
    
    /// <summary>
    /// Missing hours are hours that are supposed to be returned by prediction engine and that do not exist in output
    /// </summary>
    /// <param name="referenceDate">Calculation date</param>
    /// <param name="reportedDateTimes">Reported time stamps by the engine</param>
    /// <param name="thresholds">Validation thresholds as configured in application startup config</param>
    private static List<ValidationResult> GenerateUnrelatedHoursValidation(
        DateOnly referenceDate, 
        List<DateTime> reportedDateTimes,
        TimeStampsThresholdsConfig thresholds)
    {
        var validations = new List<ValidationResult>();
        var unrelatedDateTimes = reportedDateTimes
            .Where(d => DateOnly.FromDateTime(d) != referenceDate)
            .ToList();
        // ---------- Unrelated timestamps validation ----------
        if (unrelatedDateTimes.Any())
        {
            var status = thresholds.EnforceErrorOnUnrelatedTimeStamps
                ? ValidationStatus.Error
                : ValidationStatus.Warning;

            validations.Add(new ValidationResult(
                "UnrelatedHoursToReferenceDate",
                status,
                $"Found timestamps unrelated to reference date. " +
                $"Count: {unrelatedDateTimes.Count}. " +
                $"Timestamps: {string.Join(',', unrelatedDateTimes)}. " +
                $"ReferenceDate: {referenceDate}"
            ));
        }
        return validations;
    }
    
    /// <summary>
    /// Missing hours are hours that are supposed to be returned by prediction engine and that do not exist in output
    /// </summary>
    /// <param name="referenceDate">Calculation date</param>
    /// <param name="thresholds">Validation thresholds as configured in application startup config</param>
    /// <param name="reportedHours">Hours as reported by the prediction engine</param>
    private static List<ValidationResult> GenerateMissingHoursValidation(
        DateOnly referenceDate, 
        List<int> reportedHours,
        TimeStampsThresholdsConfig thresholds)
    {
        var validations = new List<ValidationResult>();
        var missingHours = Enumerable
            .Range(0, ExpectedHoursPerDay)
            .Where(h => !reportedHours.Contains(h))
            .ToList();
        // ---------- Missing hours validation ----------
        if (missingHours.Any())
        {
            var missingPercentage =
                (missingHours.Count * 100d) / ExpectedHoursPerDay;

            var status = missingPercentage >
                         thresholds.MaxToleratedMissingHoursPercentage
                ? ValidationStatus.Error
                : ValidationStatus.Warning;

            validations.Add(new ValidationResult(
                "MissingHours",
                status,
                $"Missing {missingHours.Count}/{ExpectedHoursPerDay} hours " +
                $"({missingPercentage:F2}%). " +
                $"Threshold: {thresholds.MaxToleratedMissingHoursPercentage}%. " +
                $"Missing hours: {string.Join(',', missingHours)}. " +
                $"ReferenceDate: {referenceDate}"
            ));
        }
        return validations;
    }
}