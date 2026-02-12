using WeatherInsights.Collector.Domain.AggregateModel.Insight;

namespace WeatherInsights.Collector.Domain.Ports.Repositories;

/// <summary>
/// Repository pattern
/// Managing sb access for <see cref="WfInsight"/> entity
/// </summary>
public interface IWfInsightRepository
{
    /// <summary>
    /// Saves the provided list of wf insights
    /// </summary>
    /// <param name="wfInsights">list of wf insights to insert</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task Save(IEnumerable<WfInsight> wfInsights, CancellationToken cancellationToken);
    
    /// <summary>
    /// Queries database and get insights between the provided date times.
    /// A datetime starts from hour 00:00:00
    /// </summary>
    /// <param name="cityId">city id concerned by insight</param>
    /// <param name="fromDateTime">Inferior query filter.</param>
    /// <param name="toDateTime">Superior query filter.</param>
    /// <param name="cancellationToken">Propagate for database calls cancellation.</param>
    /// <returns></returns>
    Task<List<WfInsight>> GetInsights(int cityId, DateTime fromDateTime, DateTime toDateTime, CancellationToken cancellationToken);
}