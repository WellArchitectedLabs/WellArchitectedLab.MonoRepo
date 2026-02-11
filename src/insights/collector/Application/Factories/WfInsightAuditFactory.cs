using System.Text.Json;
using WeatherInsights.Collector.Domain.AggregateModel.Audit;
using WeatherInsights.Collector.Domain.AggregateModel.Insight.Extensions;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;

namespace WeatherInsights.Collector.Application.Factories;


/// <summary>
/// Factory methods for <see cref="WfInsightAudit"/>
/// </summary>
public static class WfInsightAuditFactory
{
    /// <summary>
    /// Creates an audit object from the engine execution requests and responses
    /// </summary>
    /// <param name="wfEngineInput"></param>
    /// <param name="wfEngineOutput"></param>
    /// <param name="insightId"></param>
    /// <returns></returns>
    public static WfInsightAudit CreateFromWfInsight(
        WfEngineInput wfEngineInput, 
        WfEngineOutput wfEngineOutput, 
        int? insightId)
            => new WfInsightAudit
                {
                    RequestTimeUtc = wfEngineInput.RequestTime,
                    ResponseTimeUtc = wfEngineOutput.ResponseTime,
                    WeatherEngineInput = JsonSerializer.Serialize(wfEngineInput),
                    WeatherEngineOutput = JsonSerializer.Serialize(wfEngineOutput),
                    WeatherInsightId = insightId
                };
}