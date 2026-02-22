using MasterData.Domain.AggregateModel.Actuals;
using MasterData.Domain.AggregateModel.Actuals.Enums;

namespace MasterData.Application.Services.Interfaces;

/// <summary>
/// Management layer for <see cref="WfActual"/> entity
/// </summary>
public interface IWfActualService
{
    /// <summary>
    /// Returns the historical dates wf actual data based on the provided parameters.
    /// Method would also apply validation logic on the parameters in order for slicing to be correctly applied. 
    /// </summary>
    /// <param name="referenceDate">date subject of forecasting</param>
    /// <param name="historicalDepthYears">number of past years of history returned for prediction analysis</param>
    /// <param name="rollingWindowDays">odd number representing the number of days to pick in every historical year.</param>
    /// <param name="leapDayResolutionStrategy">resolving strategy for leap years (29th of February)</param>
    /// <param name="cancellationToken">provide cancellation token for stopping canceled processes down to downstream calls</param>
    /// <returns></returns>
    Task<IReadOnlyCollection<WfActual>> GetHistoricalSlice(DateOnly referenceDate,
        int historicalDepthYears,
        int rollingWindowDays,
        LeapDayResolutionStrategy leapDayResolutionStrategy,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets actuals which timestamps belong to the given date range
    /// </summary>
    /// <param name="fromDate">get actuals superior to this date</param>
    /// <param name="toDate">get actuals inferior to this date</param>
    /// <param name="cancellationToken">cancellation propagation</param>
    /// <param name="cityId"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<WfActual>> GetByDateRange(
        int cityId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken);
}