using WeatherInsights.Collector.Domain.AggregateModel.Insight;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;

namespace WeatherInsights.Collector.Domain.Ports.Clients.Interfaces;

/// <summary>
/// Client interface for interacting with the weather insights engine
/// The weather insights engine is the component responsible 
/// </summary>
public interface IWeatherInsightsEngine
{
    /// <summary>
    /// Calls the weather forecasting engine with the needed input
    /// The input model is <see cref="WfEngineInput"/>
    /// The output model is <see cref="WfEngineOutput"/>
    /// </summary>
    /// <param name="wfEngineInput"></param>
    /// <returns></returns>
    Task<WfEngineOutput> Call(WfEngineInput wfEngineInput, CancellationToken cancellationToken);
}