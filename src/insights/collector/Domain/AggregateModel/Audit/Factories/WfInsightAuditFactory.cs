using System.Text.Json;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;

namespace WeatherInsights.Collector.Domain.AggregateModel.Audit.Factories;


/// <summary>
/// Factory methods for <see cref="WfInsightAudit"/>
/// </summary>
public static class WfInsightAuditFactory
{
    /// <summary>
    /// Serialization option to be relayed for json serializer
    /// </summary>
    static readonly JsonSerializerOptions SerializationOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };
    
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
                    WeatherEngineInput = JsonSerializer.Serialize(wfEngineInput, SerializationOptions),
                    WeatherEngineOutput = JsonSerializer.Serialize(wfEngineOutput, SerializationOptions),
                    WeatherInsightId = insightId
                };
}