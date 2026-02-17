using WeatherInsights.Bff.Application.Services.Interfaces;
using WeatherInsights.Bff.Domain.AggregateModel.Insights;
using WeatherInsights.Bff.Domain.Ports.Adapters;

namespace WeatherInsights.Bff.Application.Services;

/// <summary>
/// Implementation of <see cref="IWeatherInsightsService"/>
/// </summary>
public class WeatherInsightService(IWeatherInsightsCollectorAdapter wInsightAdapter) : IWeatherInsightsService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<WeatherInsight>> Get(
        int cityId, 
        DateTime from, 
        DateTime to, 
        CancellationToken cancellationToken) 
            => await wInsightAdapter.Get(cityId, from, to, cancellationToken);
}