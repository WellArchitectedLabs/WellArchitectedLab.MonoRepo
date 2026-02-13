using WeatherInsights.Collector.Application.Interfaces;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;
using WeatherInsights.Collector.Domain.Ports.Database.Repositories.Interfaces;

namespace WeatherInsights.Collector.Application;

/// <summary>
/// Implements an insights pipelines.
/// </summary>
/// <param name="wfEngineCaller"><see cref="IWfEngineCaller"/></param>
/// <param name="engineInputCollector"><see cref="IWfEngineInputCollector"/></param>
/// <param name="insightModelPersister"><see cref="IWfInsightModelPersister"/></param>
/// <param name="insightMonitor"><see cref="IWfInsightMonitor"/></param>
public class WfInsightPipeline(
    IWfEngineCaller wfEngineCaller,
    IWfEngineInputCollector engineInputCollector,
    IWfInsightModelPersister insightModelPersister,
    IWfInsightMonitor insightMonitor,
    IWfInsightRepository wfInsightRepository) : IWfInsightPipeline
{
    /// <inheritdoc/>
    public async Task LaunchPrediction(DateOnly referenceDate, CancellationToken cancellationToken)
    {
        var perCityEngineInputs = await engineInputCollector.CollectPredictionInput(referenceDate, cancellationToken);
        var perCityEngineOutputs = await wfEngineCaller.CallEngine(perCityEngineInputs, cancellationToken);
        var monitoringReport = await insightMonitor.ApplyValidationAndLogs(referenceDate, perCityEngineOutputs, cancellationToken);
        if (monitoringReport.Any(validation => validation.HasErrors))
            // interrupt execution in any case of error
            throw new ApplicationException("An internal system inconsistencies prevented server from relaying the calculation. Please refer to logs for more details.");
        await insightModelPersister.PersistModel(perCityEngineInputs, perCityEngineOutputs, cancellationToken);
    }
    
    /// <inheritdoc/>
    public Task<List<WfInsight>> GetInsights(int cityId, DateTime fromDateTime, DateTime toDateTime, CancellationToken cancellationToken) =>
            wfInsightRepository.GetInsights(cityId, fromDateTime, toDateTime, cancellationToken);
    
}