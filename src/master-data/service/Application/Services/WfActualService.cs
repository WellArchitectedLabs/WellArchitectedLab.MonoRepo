using MasterData.Application.Services.Interfaces;
using MasterData.Domain.AggregateModel.Actuals;
using MasterData.Domain.AggregateModel.Actuals.Enums;
using MasterData.Domain.AggregateModel.Actuals.Extensions;
using MasterData.Domain.AggregateModel.Actuals.ValuesObjects;
using MasterData.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace MasterData.Application.Services;

/// <summary>
/// Implementation management layer for weather forecast actuals
/// </summary>
public class WfActualService(IWfActualRepository wfActualRepository, ILogger<WfActualService> logger) : IWfActualService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<WfActual>> GetHistoricalSlice(
        DateOnly referenceDate,
        int historicalDepthYears,
        int rollingWindowDays,
        LeapDayResolutionStrategy leapDayResolutionStrategy,
        CancellationToken cancellationToken)
    {
        var referenceDateValue = new ReferenceDate(referenceDate);
        var toBeFetchedDateTimes = referenceDateValue.ResolveHistoricalDateTimes(
            historicalDepthYears,
            rollingWindowDays,
            leapDayResolutionStrategy
        );

        var actuals = await wfActualRepository.GetByDateTimes(
            toBeFetchedDateTimes, cancellationToken);

        LogMissingTimestamps(toBeFetchedDateTimes, actuals.Select(a => a.TimestampUtc).ToList());

        return actuals;
    }
    
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<WfActual>> GetByDateRange(
        int cityId,
        DateTime fromDate, 
        DateTime toDate,
        CancellationToken cancellationToken)
    {
        var toBeFetchedDateTimes = fromDate.GetHoursUpTo(toDate);
        
        var actuals = await wfActualRepository.GetByDateTimes(
            cityId,
            toBeFetchedDateTimes, 
            cancellationToken);

        LogMissingTimestamps(toBeFetchedDateTimes, actuals.Select(a => a.TimestampUtc).ToList());

        return actuals; 
    }


    /// <summary>
    /// Logs missing timestamps
    /// </summary>
    /// <param name="requested"></param>
    /// <param name="actuals"></param>
    private void LogMissingTimestamps(
        IReadOnlyCollection<DateTime> requested,
        IReadOnlyCollection<DateTime> actuals)
    {
        var found = actuals
            .ToHashSet();

        foreach (var ts in requested)
        {
            if (!found.Contains(ts))
            {
                logger.LogWarning(
                    "Weather actual missing for timestamp {TimestampUtc}",
                    ts);
            }
        }
    }
}