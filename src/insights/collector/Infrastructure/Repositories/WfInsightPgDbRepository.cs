using WeatherInsights.Collector.Domain.AggregateModel.Insight;
using WeatherInsights.Collector.Domain.Ports.Repositories;

namespace WeatherInsights.Collector.Infrastructure.Repositories;

/// <summary>
/// Postgres implementation of <see cref="WfInsight"/> entity data access
/// </summary>
public class WfInsightPgDbRepository : IWfInsightRepository
{
    /// <inheritdoc/>
    public Task Save(IEnumerable<WfInsight> wfInsights, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc/>
    public Task<List<WfInsight>> GetInsights(int cityId, DateTime fromDateTime, DateTime toDateTime, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}