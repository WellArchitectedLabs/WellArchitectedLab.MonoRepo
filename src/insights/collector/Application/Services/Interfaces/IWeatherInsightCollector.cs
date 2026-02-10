namespace WeatherInsights.Collector.Application.Services.Interfaces;

using Domain.AggregateModel.Insight;

/// <summary>
/// Data science engine needs the set of data that is stored in other services
/// Collector's responsibility is to aggregate this data, unify it, clean it, validate it and send it to the prediction engine
/// </summary>
public interface IWeatherInsightCollector
{
    /// <summary>
    /// Aggregates a weather insight historical details from the system
    /// Then posts and weather insight payload to the forecasting engine
    /// </summary>
    /// <param name="referenceDate">the reference calculation date</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task CollectPrediction(DateOnly referenceDate, CancellationToken cancellationToken);
}