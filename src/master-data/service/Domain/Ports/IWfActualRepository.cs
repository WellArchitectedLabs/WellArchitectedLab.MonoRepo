using MasterData.Domain.AggregateModel.Actuals;

namespace MasterData.Domain.Ports;

/// <summary>
/// Repository around <see cref="WfActual"/> entity
/// </summary>
public interface IWfActualRepository
{
    Task<IReadOnlyCollection<WfActual>> GetByDateTimes(
        IReadOnlyCollection<DateTime> timestampsUtc,
        CancellationToken ct = default);
}