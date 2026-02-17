using WeatherInsights.Bff.Domain.AggregateModel.Insights;
using WeatherInsights.Bff.Domain.Ports.Adapters;
using WeatherInsights.Collector.Client;
using WeatherInsights.Collector.Client.Dtos.Parameters.Get;

namespace WeatherInsights.Bff.Infrastructure.Adapters;

/// <summary>
/// Wrapper around <see cref="IWeatherInsightsClient"/>
/// </summary>
public class HttpWeatherInsightsCollectorAdapter
    (IWeatherInsightsClient weatherInsightsHttpClient): IWeatherInsightsCollectorAdapter
{
    /// <inheritdoc/>
    public async Task<IEnumerable<WeatherInsight>> Get(int cityId, DateTime from, DateTime to, CancellationToken ct)
    {
        var getWfInsightDto = await weatherInsightsHttpClient.Get(cityId,
            new GetWeatherInsightParameters
            {
                FromDateTime = from,
                ToDateTime = to
            },
            ct);

        return getWfInsightDto.Select(wfDto => new WeatherInsight
        {
            CityId = wfDto.CityId,
            Temperature = wfDto.Temperature,
            WindSpeed = wfDto.WindSpeed,
            TimestampUtc = wfDto.TimestampUtc,
            Precipitation =  wfDto.Precipitation
        });
    }
}