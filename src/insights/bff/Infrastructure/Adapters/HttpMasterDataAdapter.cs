using MasterData.Client;
using WeatherInsights.Bff.Domain.AggregateModel.City;
using WeatherInsights.Bff.Domain.Ports.Adapters;

namespace WeatherInsights.Bff.Infrastructure.Adapters;

/// <summary>
/// Wrapper around <see cref="IMasterDataClient"/>
/// </summary>
public class HttpMasterDataAdapter(IMasterDataClient masterDataHttpClient) : IMasterDataAdapter
{
    /// <inheritdoc/>
    public async Task<List<City>> GetAllManagedCities(CancellationToken cancellationToken)
    {
        var cityDtos = await masterDataHttpClient.GetAllCities();
        return cityDtos.Select(cDto => new City
        {
            Id = cDto.Id,
            Name = cDto.Name
        }).ToList();
    }
}