namespace MasterData.Domain.AggregateModel.Actuals.Extensions;


/// <summary>
/// Domain driven extensions around <see cref="DateTime"/>
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns all hours (00 minutes, 00 seconds datetime with variable hours)
    /// From the extended datetime to the given to date in parameter of method
    /// </summary>
    /// <param name="fromDateTime">from date time</param>
    /// <param name="toDateTime">to datetime</param>
    /// <returns></returns>
    public static IReadOnlyCollection<DateTime> GetHoursUpTo(this DateTime fromDateTime, DateTime toDateTime)
    {
        if (toDateTime < fromDateTime)
            return Array.Empty<DateTime>();

        // Normalize both to the beginning of their respective hours
        var start = new DateTime(
            fromDateTime.Year,
            fromDateTime.Month,
            fromDateTime.Day,
            fromDateTime.Hour,
            0,
            0,
            DateTimeKind.Utc);

        if (fromDateTime.Minute > 0 || fromDateTime.Second > 0)
            // special case for the when user provides a non-zero minute or second
            // in this case, we want to exclude the current hour since it does not align with the hour boundaries
            start = start.AddHours(1);

        var end = new DateTime(
            toDateTime.Year,
            toDateTime.Month,
            toDateTime.Day,
            toDateTime.Hour,
            0,
            0,
            DateTimeKind.Utc);

        var results = new List<DateTime>();

        for (var current = start; current <= end; current = current.AddHours(1))
        {
            results.Add(current);
        }

        return results;
    }
    
}