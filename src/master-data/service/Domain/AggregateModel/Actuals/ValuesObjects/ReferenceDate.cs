using MasterData.Domain.AggregateModel.Actuals.Enums;

namespace MasterData.Domain.AggregateModel.Actuals.ValuesObjects;

/// <summary>
/// Value object that represents a reference date
/// </summary>
public record ReferenceDate
{
    /// <summary>
    /// Date value
    /// </summary>
    private DateOnly Value { get; init; }
    
    /// <summary>
    /// Instantiates the date value from the current UTC datetime
    /// </summary>
    /// <param name="referenceDate">the reference date value</param>
    public ReferenceDate(DateOnly referenceDate)
    {
        Value =  referenceDate;
    }
    
    /// <summary>
    /// Returns a sorted historical dates list based on the provided parameters
    /// </summary>
    /// <param name="historicalDepthYears">number of years of depth</param>
    /// <param name="rollingWindowDays">odd number representing the slicing window</param>
    /// <param name="resolutionStrategy">leap year resolving strategy</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IReadOnlyCollection<DateTime> ResolveHistoricalDateTimes(
         int historicalDepthYears,
         int rollingWindowDays,
         LeapDayResolutionStrategy  resolutionStrategy)
    {
        var referenceDate = Value;
        if (historicalDepthYears <= 0)
            throw new ArgumentOutOfRangeException(nameof(historicalDepthYears));

        if (rollingWindowDays <= 0 || rollingWindowDays > 30 || rollingWindowDays % 2 == 0)
            throw new ArgumentOutOfRangeException(nameof(rollingWindowDays),
                "Rolling window must be an odd number between 1 and 30.");
        
        // Get past years history
        var pastYearsHistoricalDates = ResolvePastYearsHistoricalDateTimes(
            historicalDepthYears, 
            rollingWindowDays, 
            resolutionStrategy, 
            referenceDate);
        
        // get near history (N days before the current date)
        var nearHistoryDateTimes = ResolveNearHistoryDateTimes(
            rollingWindowDays, referenceDate);
        
        // Concat near and past history dates
        var historicalDateTimes = pastYearsHistoricalDates
            .Union(nearHistoryDateTimes);

        // Only we calculate the historical dates based on the consumer parameters
        // We Project them to hourly slices
        // And this, because we manage weather actuals on hourly base
        return historicalDateTimes.SelectMany
            (GenerateHourlySlices)
            .ToList();
    }

    private static IEnumerable<DateOnly> ResolveNearHistoryDateTimes(int rollingWindowDays, DateOnly referenceDate)
    {
        var nearHistoryDateTimes = Enumerable.Range(1, rollingWindowDays)
            .Select(day => referenceDate.AddDays(-day));
        return nearHistoryDateTimes;
    }

    private static SortedSet<DateOnly> ResolvePastYearsHistoricalDateTimes(int historicalDepthYears, int rollingWindowDays,
        LeapDayResolutionStrategy resolutionStrategy, DateOnly referenceDate)
    {
        var historicalDates = new SortedSet<DateOnly>();
        var offsetDays = (rollingWindowDays - 1) / 2;
        
        for (var yearOffset = 1; yearOffset <= historicalDepthYears; yearOffset++)
        {
            var targetYear = referenceDate.Year - yearOffset;

            var resolvedReference = ResolveHistoricalReferenceDate(
                referenceDate,
                targetYear,
                resolutionStrategy);

            for (var delta = -offsetDays; delta <= offsetDays; delta++)
            {
                historicalDates.Add(resolvedReference.AddDays(delta));
            }
        }

        return historicalDates;
    }


    /// <summary>
    /// Retrieves historical date based on the given strategy dans year
    /// If year is a leap year => we fall back to a date based on the resolver strategy
    /// If year is a non leap year => reference date param will be returned as is.
    /// </summary>
    /// <param name="referenceDate">base reference date</param>
    /// <param name="targetYear">to be resolved date year</param>
    /// <param name="resolutionStrategy">resolution strategy</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static DateOnly ResolveHistoricalReferenceDate(
        DateOnly referenceDate,
        int targetYear,
        LeapDayResolutionStrategy resolutionStrategy)
    {
        // Non-leap-day: trivial case
        if (referenceDate is not { Month: 2, Day: 29 })
            // break algo for regular dates
            return new DateOnly(targetYear, referenceDate.Month, referenceDate.Day);

        // Leap-day reference (Feb 29)
        if (DateTime.IsLeapYear(targetYear))
            return new DateOnly(targetYear, 2, 29);

        return resolutionStrategy switch
        {
            LeapDayResolutionStrategy.ClampToFeb28 =>
                new DateOnly(targetYear, 2, 28),

            LeapDayResolutionStrategy.ShiftToMar01 =>
                new DateOnly(targetYear, 3, 1),

            _ => throw new ArgumentOutOfRangeException(nameof(resolutionStrategy))
        };
    }
    
    /// <summary>
    /// Generate hourly slices from a date only
    /// </summary>
    /// <param name="historicalDate">historical date to generate slices from</param>
    /// <returns></returns>
    private IEnumerable<DateTime> GenerateHourlySlices(DateOnly historicalDate)
        =>
            Enumerable.Range(0, 24).Select(hour =>
                new DateTime(
                    historicalDate.Year, 
                    historicalDate.Month, 
                    historicalDate.Day, hour, 0, 0));
   
}