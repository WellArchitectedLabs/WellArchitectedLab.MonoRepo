using Microsoft.Extensions.Logging;
using WeatherInsights.Collector.Domain.AggregateModel.Technical;
using WeatherInsights.Collector.Domain.AggregateModel.Technical.Enums;
using WeatherInsights.Collector.Domain.AggregateModel.Technical.Visitors;

namespace WeatherInsights.Collector.Domain.Ports.Clients.Models;

/// <summary>
/// The output returned from prediction engine
/// </summary>
public record WfEngineOutput
{
    /// <summary>
    /// City id concerned by calculation
    /// </summary>
    public required int CityId { get; init; }
    /// <summary>
    /// Reference date concerned by calculation
    /// </summary>
    public required DateOnly ReferenceDate { get; init; }
    
    /// <summary>
    /// Engine responded on this date time
    /// </summary>
    public DateTime ResponseTime = DateTime.UtcNow;
    
    /// <summary>
    /// A prediction indexed by hour of the reference date
    /// </summary>
    public required IDictionary<DateTime, WfEngineInsightOutputItem> PerHourPrediction { get; init; }
    
    
    /// <summary>
    /// Validates the wf engine output object and returns the unified validation object.
    /// Accepts the reference date
    /// </summary>
    /// <param name="referenceDate"></param>
    /// <returns></returns>
    public IEnumerable<ValidationResult> Validate(DateOnly referenceDate)
    {
        var hours = Enumerable.Range(0, 23).ToList();
        var calculationDateTimes = PerHourPrediction.Keys.ToHashSet();
        var calculationHours = calculationDateTimes.Select(d => d.Hour).ToHashSet();
        var nonReportedHours = new List<int>();
        var nonRelatedDateTimesToReference = new List<DateTime>();
        var validations = new List<ValidationResult>();
        foreach (var hour in hours)
        {
            if(!calculationHours.Contains(hour))
                nonReportedHours.Add(hour);
        }

        foreach (var calculationDateTime in calculationDateTimes)
        {
            if(DateOnly.FromDateTime(calculationDateTime) != referenceDate)
                nonRelatedDateTimesToReference.Add(calculationDateTime);
        }

        if (nonReportedHours.Any())
        {
            validations.Add(new ValidationResult(
                "MissingHours",
                ValidationStatus.Warning,
                $"Some reference date hours are not present into engine result. " +
                $"Non reported hours list: {string.Join(',', nonReportedHours)}" +
                $"Full DateTime List:  {string.Join(',', calculationHours)}." +
                $"\r\nReferenceDate: {referenceDate}"));
        }
        
        if(nonRelatedDateTimesToReference.Any())
        {
            validations.Add(new ValidationResult(
                "UnrelatedHoursToReferenceDate",
                // this is a hard error
                // a single non-related data invalidates all engine outputs 
                ValidationStatus.Error,
                $"Some reported hours by the prediction engine are unrelated to reference date. " +
                $"Non related hours list: {string.Join(',', nonRelatedDateTimesToReference)}." +
                $"Full DateTime List:  {string.Join(',', calculationHours)}." +
                $"\r\nReferenceDate: {referenceDate}"));
        }
        
        if (PerHourPrediction.Keys.Count != hours.Count())
        {
            validations.Add(new ValidationResult(
                "UnmatchedHoursCount",
                ValidationStatus.Warning,
                $"The reported number of hours for the reference date by the prediction engine is not equal to {hours.Count()} hours." +
                $"Hours count:  {PerHourPrediction.Keys.Count}." +
                $"Full DateTime List:  {string.Join(',', calculationHours)}." +
                $"\r\nReferenceDate: {referenceDate}"));
        }

        return validations;
    }
}

/// <summary>
/// Output for a single timeStamp (date + exact hour)
/// </summary>
public record WfEngineInsightOutputItem
{
    /// <summary>
    /// Predicted temperature in Celsius
    /// </summary>
    public required decimal Temperature { get; init; }
    /// <summary>
    /// Predicted wind speed
    /// </summary>
    public required decimal WindSpeed { get; init; }
    /// <summary>
    /// Predicted Precipitation
    /// </summary>
    public required decimal Precipitation { get; init; }
}