using WeatherInsights.Bff.Domain.AggregateModel.Insights;

namespace WeatherInsights.Bff.Application.Services.Interfaces;

/// <summary>
/// Management layer for <see cref="WeatherInsight"/> entity
/// </summary>
public interface IWeatherInsightsService
{
    /// <summary>
    /// Fetches weather insights based on the given filters
    /// </summary>
    /// <param name="cityId">City id as communicated by frontend.</param>
    /// <param name="from">from date in a date time format</param>
    /// <param name="to">to date in a date time format</param>
    /// <param name="cancellationToken">propagate frontend cancellations to all called APIs.</param>
    /// <returns></returns>
    Task<List<WeatherInsight>> Get(int cityId, DateTime from, DateTime to, CancellationToken cancellationToken);
}
