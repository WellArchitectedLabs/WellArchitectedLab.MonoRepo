using MasterData.Application.Services.Interfaces;
using MasterData.Domain.AggregateModel.Actuals;
using MasterData.Domain.AggregateModel.Actuals.Enums;
using MasterData.Domain.AggregateModel.Actuals.ValuesObjects;
using MasterData.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace MasterData.Application.Services;

/// <summary>
/// Implementation management layer for weather forecast actuals
/// </summary>
public class WfActualService(IWfActualRepository wfActualRepository, ILogger<WfActualService> logger) : IWfActualService
{
    public async Task<IReadOnlyCollection<WfActual>> GetHistoricalSlice(
        DateOnly referenceDate,
        int historicalDepthYears,
        int rollingWindowDays,
        LeapDayResolutionStrategy leapDayResolutionStrategy,
        CancellationToken cancellationToken)
    {
        var referenceDateDomainObject = new ReferenceDate(referenceDate);
        var toBeFetchedDateTimes = referenceDateDomainObject.ResolveHistoricalDateTimes(
            historicalDepthYears,
            rollingWindowDays,
            leapDayResolutionStrategy
        );

        var actuals = await wfActualRepository.GetByDateTimes(
            toBeFetchedDateTimes, cancellationToken);

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