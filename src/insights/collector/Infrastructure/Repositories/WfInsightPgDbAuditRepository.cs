using WeatherInsights.Collector.Domain.AggregateModel.Audit;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;
using WeatherInsights.Collector.Domain.Ports.Repositories;

namespace WeatherInsights.Collector.Infrastructure.Repositories;

public class WfInsightPgDbAuditRepository : IWfInsightAuditRepository
{
    public Task Save(IEnumerable<WfInsightAudit> wfInsights, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}