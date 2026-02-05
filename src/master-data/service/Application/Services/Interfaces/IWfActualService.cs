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
    /// <param name="referenceDate"></param>
    /// <param name="historicalDepthYears"></param>
    /// <param name="rollingWindowDays"></param>
    /// <param name="leapDayResolutionStrategy"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<WfActual>> GetHistoricalSlice(DateOnly referenceDate,
        int historicalDepthYears,
        int rollingWindowDays,
        LeapDayResolutionStrategy leapDayResolutionStrategy,
        CancellationToken cancellationToken);
}