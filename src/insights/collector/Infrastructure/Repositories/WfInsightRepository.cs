using WeatherInsights.Collector.Domain.AggregateModel.Insight;
using WeatherInsights.Collector.Domain.Ports.Repositories;

namespace WeatherInsights.Collector.Infrastructure.Repositories;

public class WfInsightRepository : IWfInsightDbRepository
{
    public Task Save(IEnumerable<WfInsight> wfInsights, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}