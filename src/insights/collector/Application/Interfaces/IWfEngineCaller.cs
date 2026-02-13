using WeatherInsights.Collector.Domain.Ports.HttpClients.Models;

namespace WeatherInsights.Collector.Application.Interfaces;

/// <summary>
/// Calls engine given an input
/// Returns a unified output object
/// </summary>
public interface IWfEngineCaller
{
    /// <summary>
    /// We call the prediction engine from the collected input
    /// The engine call results with an output that we store as audit
    /// And from which we can map an insight object that would be consumed by consumers.
    /// Returns a map of prediction outputs by city id
    /// </summary>
    /// <param name="perCityEngineInputs">a map of a prediction input per city<see cref="WfEngineInput"/></param>
    /// <param name="cancellationToken">needed for cancellation propagation. Please provide when available.</param>
    /// <returns>a per city output engine map</returns>
    Task<IDictionary<int, WfEngineOutput>> CallEngine(
        IDictionary<int, WfEngineInput> perCityEngineInputs,
        CancellationToken cancellationToken);
}