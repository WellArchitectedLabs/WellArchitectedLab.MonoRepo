using WeatherInsights.Bff.Domain.AggregateModel.City;

namespace WeatherInsights.Bff.Application.Services.Interfaces;

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