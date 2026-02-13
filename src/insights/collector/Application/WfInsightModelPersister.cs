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
            await wfInsightRepository.Save(perCityInsights.Values.SelectMany(wIns => wIns), cancellationToken);
            var cityIds = perCityInsights.Keys.Select(k => k).ToHashSet();
            // since the unit of work instance is scoped, both insights and insight audits
            // repositories will use the same instance created above
            var wfInsightAudits = cityIds.SelectMany(cityId =>
                // we project audit by concerned insights
                perCityInsights[cityId].Select(wfIns =>
                    WfInsightAuditFactory.CreateFromWfInsight(perCityEngineInputs[cityId], perCityEngineOutputs[cityId],
                        wfIns.Id)));
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