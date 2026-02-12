using WeatherInsights.Collector.Domain.AggregateModel.Insight;

namespace WeatherInsights.Collector.Application.Interfaces;

/// <summary>
/// Pipelines code for kicking predictions in interaction with prediction engine.
/// </summary>
public interface IWfInsightPipeline
{
    /// <summary>
    /// Coordinates / chains calls to different services in order to ensure
    /// - Engine inputs are valid and that drifts are logged via logger.
    /// - Prediction engine is called (or what we call the Wf engine ot the weather forecast engine) via engine caller.
    /// - Engine outputs are valid and that anomalies are logged via logger.
    /// - Audits and insights objects are stored in data stores via persister. 
    /// </summary>
    /// <param name="referenceDate">calculation date</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>, procured from caller.</param>
    /// <returns></returns>
    public Task LaunchPrediction(DateOnly referenceDate, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets the predictions calculated for the given reference date.
    /// Methods returns hourly prediction for each (please count )
    /// </summary>
    /// <param name="cityId">City concerned by insights.</param>
    /// <param name="fromDateTime">Get predictions from this date time.</param>
    /// <param name="toDateTime">Get predictions calculated to this date time.</param>
    /// <param name="cancellationToken">Please propagate from caller.</param>
    /// <returns></returns>
    public Task<List<WfInsight>> GetInsights(int cityId, DateTime fromDateTime, DateTime toDateTime, CancellationToken cancellationToken);
}