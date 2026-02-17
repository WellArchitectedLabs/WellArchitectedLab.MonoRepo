using WeatherInsights.Collector.Domain.AggregateModel.Audit;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;

namespace WeatherInsights.Collector.Domain.Ports.Database.Repositories.Interfaces;

/// <summary>
/// Repository pattern
/// Managing sb access for <see cref="WfInsight"/> entity
/// </summary>
public interface IWfInsightAuditRepository
{
    /// <summary>
    /// Saves the provided list of wf insights
    /// </summary>
    /// <param name="wfInsights">list of wf insights to insert</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task Save(IEnumerable<WfInsightAudit> wfInsights, CancellationToken cancellationToken);
}