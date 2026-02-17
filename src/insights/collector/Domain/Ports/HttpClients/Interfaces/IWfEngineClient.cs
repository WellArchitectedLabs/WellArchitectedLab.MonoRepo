using WeatherInsights.Collector.Domain.Ports.HttpClients.Models;

namespace WeatherInsights.Collector.Domain.Ports.HttpClients.Interfaces;

/// <summary>
/// Client interface for interacting with the weather insights engine
/// The weather insights engine is the component responsible 
/// </summary>
public interface IWfEngineClient
{
    /// <summary>
    /// Calls the weather forecasting engine with the needed input
    /// The input model is <see cref="WfEngineInput"/>
    /// The output model is <see cref="WfEngineOutput"/>
    /// </summary>
    /// <param name="wfEngineInput">Weather forecast input, collected from available sources.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<WfEngineOutput> Call(WfEngineInput wfEngineInput, CancellationToken cancellationToken);
}