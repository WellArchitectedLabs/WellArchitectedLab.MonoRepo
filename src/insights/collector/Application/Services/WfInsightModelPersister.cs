using WeatherInsights.Collector.Application.Factories;
using WeatherInsights.Collector.Application.Services.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;
using WeatherInsights.Collector.Domain.Ports.Repositories;

namespace WeatherInsights.Collector.Application.Services;

/// <summary>
/// Implements the wf insight persister by storing payloads into audit database
/// A more meaningful choice could be to store prediction inputs / outputs (so what we call audits)
/// Into a storage account in Azure.
/// </summary>
/// <param name="wfInsightDbRepository">persists wf insights into db repository</param>
/// <param name="wfInsightAuditRepository">persists wf insights audit into db repository</param>
public class WfInsightModelPersister(
    IWfInsightDbRepository wfInsightDbRepository,
    IWfInsightAuditRepository wfInsightAuditRepository) : IWfInsightModelPersister
{
    /// <inheritdoc/>
    public async Task PersistModel(
        IDictionary<int, WfEngineInput> perCityEngineInputs,
        IDictionary<int, WfEngineOutput> perCityEngineOutputs, 
        CancellationToken cancellationToken)
    {
        var perCityInsights = perCityEngineOutputs.ToDictionary(kv => kv.Key, kv => WfInsightFactory.CreateFromEngineResponse(kv.Value));
        await wfInsightDbRepository.Save(perCityInsights.Values.SelectMany(wIns => wIns), cancellationToken);
        var cityIds = perCityInsights.Keys.Select(k => k).ToHashSet();
        var wfInsightAudits = cityIds.SelectMany(
            cityId => 
                // we project audit by concerned insights
                perCityInsights[cityId].Select(wfIns => WfInsightAuditFactory.CreateFromWfInsight(perCityEngineInputs[cityId], perCityEngineOutputs[cityId], wfIns.Id)));
        await wfInsightAuditRepository.Save(wfInsightAudits, cancellationToken);
    }
}