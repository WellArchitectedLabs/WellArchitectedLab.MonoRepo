using WeatherInsights.Collector.Domain.Ports.HttpClients.Models;

namespace WeatherInsights.Collector.Application.Interfaces;

/// <summary>
/// Data science engine needs the set of data that is stored in other services
/// Collector's responsibility is to aggregate this data, unify it, clean it, validate it and send it to the prediction engine
/// </summary>
public interface IWfEngineInputCollector
{
    /// <summary>
    /// Aggregates a weather insight historical details from the system
    /// Then posts and weather insight payload to the forecasting engine
    /// </summary>
    /// <param name="referenceDate">the reference calculation date</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<IDictionary<int, WfEngineInput>> CollectPredictionInput(
        DateOnly referenceDate, 
        CancellationToken cancellationToken);
}