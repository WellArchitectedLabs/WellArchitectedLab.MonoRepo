using WeatherInsights.Bff.Application.Services.Interfaces;
using WeatherInsights.Bff.Domain.AggregateModel.Insights;

namespace WeatherInsights.Bff.Application.Services;

/// <summary>
/// Implementation of <see cref="IWeatherInsightsService"/>
/// </summary>
public class WeatherInsightService : IWeatherInsightsService
{
    /// <inheritdoc/>
    public Task<List<WeatherInsight>> Get(int cityId, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}