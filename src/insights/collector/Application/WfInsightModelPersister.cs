using WeatherInsights.Collector.Application.Interfaces;
using WeatherInsights.Collector.Domain.AggregateModel.Audit.Factories;
using WeatherInsights.Collector.Domain.AggregateModel.Insight.Factories;
using WeatherInsights.Collector.Domain.Ports.Database.Repositories.Interfaces;
using WeatherInsights.Collector.Domain.Ports.HttpClients.Models;
using WeatherInsights.Collector.Domain.Ports.UnitOfWork;

namespace WeatherInsights.Collector.Application;

/// <summary>
/// Implements the wf insight persister by storing payloads into audit database
/// A more meaningful choice could be to store prediction inputs / outputs (so what we call audits)
/// Into a storage account in Azure.
/// </summary>
/// <param name="wfInsightRepository">persists wf insights into db repository</param>
/// <param name="wfInsightAuditRepository">persists wf insights audit into db repository</param>
public class WfInsightModelPersister(
    IWfInsightRepository wfInsightRepository,
    IWfInsightAuditRepository wfInsightAuditRepository,
    IUnitOfWork unitOfWork) : IWfInsightModelPersister
{
    /// <inheritdoc/>
    public async Task PersistModel(
        IDictionary<int, WfEngineInput> perCityEngineInputs,
        IDictionary<int, WfEngineOutput> perCityEngineOutputs,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginAsync(cancellationToken);

        try
        {
            var perCityInsights = perCityEngineOutputs.ToDictionary(kv => kv.Key,
                kv => WfInsightFactory.CreateFromEngineResponse(kv.Value));
            
            var insightIdsByCityId = await wfInsightRepository.Save(
                perCityInsights.Values.SelectMany(wIns => wIns), cancellationToken);

            var wfInsightAudits = insightIdsByCityId
                .SelectMany(grp => grp.Select(insightId =>
                    WfInsightAuditFactory.CreateFromWfInsight(
                        perCityEngineInputs[grp.Key],
                        perCityEngineOutputs[grp.Key],
                        insightId)));
            
            await wfInsightAuditRepository.Save(wfInsightAudits, cancellationToken);
            
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}