using MasterData.Domain.AggregateModel.Actuals;

namespace MasterData.Domain.Ports;

/// <summary>
/// Repository around <see cref="WfActual"/> entity
/// </summary>
public interface IWfActualRepository
{
    /// <summary>
    /// Gets all actuals situated on the given datetime range
    /// </summary>
    /// <param name="timestampsUtc"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<WfActual>> GetByDateTimes(
        IReadOnlyCollection<DateTime> timestampsUtc,
        CancellationToken ct = default);
    
    /// <summary>
    /// Gets all actuals situated on the given datetime range
    /// </summary>
    /// <param name="cityId">City Id</param>
    /// <param name="timestampsUtc"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<WfActual>> GetByDateTimes(
        int cityId,
        IReadOnlyCollection<DateTime> timestampsUtc,
        CancellationToken ct = default);
}