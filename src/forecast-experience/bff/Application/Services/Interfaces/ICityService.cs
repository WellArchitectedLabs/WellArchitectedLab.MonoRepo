using WfExperience.Bff.Domain.AggregateModel.City;

namespace WfExperience.Bff.Application.Services.Interfaces;

/// <summary>
/// Management layer for <see cref="City"/> entity
/// </summary>
public interface ICityService
{
    /// <summary>
    /// Queries all cities
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<City>> GetAll(CancellationToken cancellationToken);
}