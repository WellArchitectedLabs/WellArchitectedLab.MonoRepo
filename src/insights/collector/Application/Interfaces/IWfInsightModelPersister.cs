using WeatherInsights.Collector.Domain.Ports.HttpClients.Models;

namespace WeatherInsights.Collector.Application.Interfaces;

/// <summary>
/// Transforms engine outputs to valida entities
/// Then Communicates with data adapters in order to persist them for usage from external consumers.
/// </summary>
public interface IWfInsightModelPersister
{
    /// <summary>
    /// Persists Wf Insight model into database
    /// </summary>
    /// <param name="perCityEngineInputs">prediction payloads as sent to wf engine.</param>
    /// <param name="perCityEngineOutputs">predictions to be persisted into database via db repository.</param>
    /// <param name="cancellationToken">propagates async tasks cancellation.</param>
    /// <returns></returns>
    Task PersistModel(
        IDictionary<int, WfEngineInput> perCityEngineInputs,
        IDictionary<int, WfEngineOutput> perCityEngineOutputs, 
        CancellationToken cancellationToken);
}