using Microsoft.Extensions.Logging;
using WeatherInsights.Collector.Application.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Clients.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;

namespace WeatherInsights.Collector.Application;

/// <summary>
/// Mediator class communicating with the weather insights forecasting engine
/// </summary>
/// <param name="wfEngineClient"></param>
/// <param name="logger"></param>
public class WfEngineCaller(
    IWfEngineClient wfEngineClient,
    ILogger<IWfEngineCaller> logger) : IWfEngineCaller
{
    /// <inheritdoc/>
    public async Task<IDictionary<int, WfEngineOutput>> CallEngine(
        IDictionary<int, WfEngineInput> perCityEngineInputs, 
        CancellationToken cancellationToken)
    {
        var cityIds = perCityEngineInputs.Keys;
        var perCityEngineResponses = new Dictionary<int, WfEngineOutput>();
        foreach (var cityId in cityIds)
        {
            // it is meant to be sequential in order to do not overload prediction engine with sudden load
            // causing potentially a thundering herd problem and also an http port exhaustion if many calls are invoked
            // please check: https://en.wikipedia.org/wiki/Thundering_herd_problem
            try
            {
                var predictionResult = await wfEngineClient.Call(perCityEngineInputs[cityId], cancellationToken);
                perCityEngineResponses.Add(cityId, predictionResult);
            }
            catch (HttpRequestException httpRequestException)
            {
                logger.LogError(httpRequestException, httpRequestException.Message);
            }
        }

        return perCityEngineResponses;
    }
}