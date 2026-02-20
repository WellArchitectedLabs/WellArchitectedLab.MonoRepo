using WeatherInsights.Bff.Application.Services.Interfaces;
using WeatherInsights.Bff.Domain.AggregateModel.Forecast;
using WeatherInsights.Bff.Domain.Ports.Adapters;

namespace WeatherInsights.Bff.Application.Services;

/// <summary>
/// Implementation of <see cref="IWeatherInsightsService"/>
/// </summary>
public class WeatherInsightService(
    IWeatherInsightsCollectorAdapter wInsightAdapter,
    IMasterDataAdapter masterDataAdapter) : IWeatherInsightsService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<WeatherForecast>> Get(
        int cityId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken)
    {
        var yesterdaysLastSecond =
            new DateTime(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), new TimeOnly(23, 59, 59));
        var limitHistoryDate = new List<DateTime>() {to, yesterdaysLastSecond}.Min();
        
        // get actuals
        var weatherActuals = await masterDataAdapter.GetActuals(from, limitHistoryDate, cancellationToken);
        
        // get insights
        var weatherInsights = new List<WeatherForecast>();
        if (yesterdaysLastSecond < limitHistoryDate)
        {
            // get insights from the upper slice of the user request
            weatherInsights = await wInsightAdapter.Get(cityId, limitHistoryDate, to, cancellationToken);
        }
        
        return weatherActuals.Concat(weatherInsights).ToList();
    }
}