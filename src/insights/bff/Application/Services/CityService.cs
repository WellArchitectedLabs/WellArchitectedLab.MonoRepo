using WeatherInsights.Bff.Application.Services.Interfaces;
using WeatherInsights.Bff.Domain.AggregateModel.City;
using WeatherInsights.Bff.Domain.Ports.Adapters;

namespace WeatherInsights.Bff.Application.Services;

/// <summary>
/// Implementation of <see cref="ICityService"/>
/// </summary>
/// <param name="masterDataAdapter">connects to external source to manage remote cities data</param>
public class CityService(IMasterDataAdapter masterDataAdapter) : ICityService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<City>> GetAll(CancellationToken cancellationToken)
        => await masterDataAdapter.GetAllManagedCities(cancellationToken);
}