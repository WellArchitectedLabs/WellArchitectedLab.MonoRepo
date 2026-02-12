namespace WeatherInsights.Collector.Application.Services.Interfaces;

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
    public Task OrchestratePipeline(DateOnly referenceDate, CancellationToken cancellationToken);
}