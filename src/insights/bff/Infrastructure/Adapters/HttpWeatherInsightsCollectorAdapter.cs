using WeatherInsights.Bff.Domain.AggregateModel.Forecast;
using WeatherInsights.Bff.Domain.Ports.Adapters;
using WeatherInsights.Collector.Client;
using WeatherInsights.Collector.Client.Dtos.Parameters.Get;

namespace WeatherInsights.Bff.Infrastructure.Adapters;

/// <summary>
/// Wrapper around <see cref="IWeatherInsightsClient"/>
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