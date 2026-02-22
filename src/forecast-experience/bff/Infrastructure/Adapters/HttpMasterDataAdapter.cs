using MasterData.Client;
using WfExperience.Bff.Domain.AggregateModel.City;
using WfExperience.Bff.Domain.AggregateModel.Forecast;
using WfExperience.Bff.Domain.Ports.Adapters;

namespace WfExperience.Bff.Infrastructure.Adapters;

/// <summary>
/// Wrapper around <see cref="IMasterDataClient"/>
/// </summary>
public class HttpMasterDataAdapter(IMasterDataClient masterDataHttpClient) : IMasterDataAdapter
{
    /// <inheritdoc/>
    public async Task<List<City>> GetAllManagedCities(CancellationToken cancellationToken)
    {
        var cityDtos = await masterDataHttpClient.GetAllCities(cancellationToken);
        return cityDtos.Select(cDto => new City
        {
            Id = cDto.Id,
            Name = cDto.Name
        }).ToList();
    }
    
    /// <inheritdoc/>
    public async Task<List<WeatherForecast>> GetActuals(
        int cityId, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var wfActuals = await masterDataHttpClient.Search(cityId, from, to, cancellationToken);
        return wfActuals.Select(wfActual => new WeatherForecast
        {
            CityId = cityId,
            Precipitation = wfActual.Precipitation,
            Temperature = wfActual.Temperature,
            TimestampUtc = wfActual.TimestampUtc,
            WindSpeed = wfActual.WindSpeed
        }).ToList();

    }
}