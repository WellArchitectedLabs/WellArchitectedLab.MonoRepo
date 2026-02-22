using WfExperience.Bff.Domain.AggregateModel.Forecast;

namespace WfExperience.Bff.Domain.Ports.Adapters;

/// <summary>
/// Adapter for the insights collector API
/// </summary>
public interface IWeatherInsightsCollectorAdapter
{
    /// <summary>
    /// Gets a weather insight for the given date range
    /// </summary>
    /// <param name="cityId">city id as referenced in master data service</param>
    /// <param name="from">a date time from timestamp</param>
    /// <param name="to">a date time to timestamp</param>
    /// <param name="ct">propagate errors to called APIs</param>
    /// <returns>the list of weather insights in the given date range and for the given city id</returns>
    Task<IReadOnlyCollection<WeatherForecast>> Get(int cityId, DateTime from, DateTime to, CancellationToken ct);
}