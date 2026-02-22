using WfExperience.Bff.Application.Services.Interfaces;
using WfExperience.Bff.Domain.AggregateModel.Forecast;
using WfExperience.Bff.Domain.Ports.Adapters;

namespace WfExperience.Bff.Application.Services;

/// <summary>
/// Implementation of <see cref="IWeatherForecastService"/>
/// </summary>
public class WeatherForecastService(
    IWeatherInsightsCollectorAdapter wInsightAdapter,
    IMasterDataAdapter masterDataAdapter) : IWeatherForecastService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<WeatherForecast>> Get(
        int cityId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken)
    {
        var yesterdaysLastSecond =
            new DateTime(DateOnly.FromDateTime(
                DateTime.Now.AddDays(-1)), new TimeOnly(23, 59, 59));
        var limitHistoryDate = new List<DateTime>() {to, yesterdaysLastSecond}.Min();
        
        // get actuals
        var weatherActuals = await masterDataAdapter.GetActuals(
            cityId, from, limitHistoryDate, cancellationToken);
        
        // get insights
        IReadOnlyCollection<WeatherForecast> weatherInsights = new List<WeatherForecast>();
        if (yesterdaysLastSecond < limitHistoryDate)
        {
            // get insights from the upper slice of the user request
            weatherInsights = await wInsightAdapter.Get(cityId, limitHistoryDate, to, cancellationToken);
        }
        
        return weatherActuals.Concat(weatherInsights).ToList();
    }
}