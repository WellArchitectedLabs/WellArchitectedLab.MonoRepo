using WeatherInsights.Bff.Domain.AggregateModel.City;

namespace WeatherInsights.Bff.Domain.Ports.Adapters;

/// <summary>
/// Connects to external source for getting master data
/// </summary>
public interface IMasterDataAdapter
{
    /// <summary>
    /// Returns the full cities list of the master data repository
    /// </summary>
    /// <param name="cancellationToken">propagates task cancellations.</param>
    /// <returns></returns>
    Task<List<City>> GetAllManagedCities(CancellationToken cancellationToken);
}