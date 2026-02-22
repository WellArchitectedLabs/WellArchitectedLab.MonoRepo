using WeatherInsights.Collector.Client;
using WeatherInsights.Collector.Client.Dtos.Parameters.Get;
using WfExperience.Bff.Domain.AggregateModel.Forecast;
using WfExperience.Bff.Domain.Ports.Adapters;

namespace WfExperience.Bff.Infrastructure.Adapters;

/// <summary>
/// Wrapper around <see cref="IWeatherInsightsCollectorClient"/>
/// </summary>
public class HttpWeatherInsightsCollectorAdapter
    (IWeatherInsightsCollectorClient weatherInsightsHttpClient): IWeatherInsightsCollectorAdapter
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<WeatherForecast>> Get(
        int cityId, DateTime from, DateTime to, CancellationToken ct)
    {
        var getWfInsightDto = await weatherInsightsHttpClient.Get(cityId,
            new GetWeatherInsightParameters
            {
                FromDateTime = from,
                ToDateTime = to
            },
            ct);

        return getWfInsightDto.Select(wfDto => new WeatherForecast
        {
            CityId = wfDto.CityId,
            Temperature = wfDto.Temperature,
            WindSpeed = wfDto.WindSpeed,
            TimestampUtc = wfDto.TimestampUtc,
            Precipitation =  wfDto.Precipitation
        }).ToList();
    }
}