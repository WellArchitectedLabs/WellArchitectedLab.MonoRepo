using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using WeatherInsights.Collector.Client.Dtos.Responses.Get;

namespace WeatherInsights.Bff.Domain.AggregateModel.Forecast.Factories;

/// <summary>
/// Factory around <see cref="WeatherForecast"/> entity
/// </summary>
public static class WeatherForecastFactory
{
    /// <summary>
    /// Creates insights list from a list of actuals
    /// </summary>
    /// <param name="weatherActuals">actuals from master data service</param>
    /// <returns></returns>
    public static IReadOnlyCollection<WeatherForecast> CreateFromWeatherActuals(
        IEnumerable<WeatherForecast> weatherActuals)
    {
        return weatherActuals.Select(wa => new WeatherForecast
        {
            CityId = wa.CityId,
            Precipitation = wa.Precipitation,
            Temperature = wa.Temperature,
            TimestampUtc = wa.TimestampUtc,
            WindSpeed = wa.WindSpeed
        }).ToList();
    }
    
    /// <summary>
    /// Creates insights list from a list of actuals
    /// </summary>
    /// <param name="weatherInsights">insights from insights collector service</param>
    /// <returns></returns>
    public static IReadOnlyCollection<WeatherForecast> CrateFromWeatherInsights(
        IEnumerable<GetWfInsightDto> weatherInsights)
    {
        return weatherInsights.Select(wi => new WeatherForecast
        {
            CityId = wi.CityId,
            Precipitation = wi.Precipitation,
            Temperature = wi.Temperature,
            TimestampUtc = wi.TimestampUtc,
            WindSpeed = wi.WindSpeed
        }).ToList();
    }
    
}