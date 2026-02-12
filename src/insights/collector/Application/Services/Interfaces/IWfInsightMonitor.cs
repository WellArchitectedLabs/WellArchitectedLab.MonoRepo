using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using WeatherInsights.Collector.Domain.AggregateModel.Technical;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;
using WeatherInsights.Collector.Domain.Ports.Config;

namespace WeatherInsights.Collector.Application.Services.Interfaces;

/// <summary>
/// Applies validation on external services models.
/// This validation flows to grafana level as logs, stored in Loki
/// Some alerts may be triggered on some pushed logs.
/// The monitor's responsibility starts from validation to monitoring data before asserting it is safe to use.
/// </summary>
public interface IWfInsightMonitor
{
    /// <summary>
    /// Validates wf actual data based on the engine data validation rules
    /// And transforms them to structured logs
    /// </summary>
    /// <param name="referenceDate">requested engine calculation date</param>
    /// <param name="engineCallConfig">necessary for wf actuals cardinality validation</param>
    /// <param name="wfActuals">wf actuals, as a result to master data service call</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<List<ValidationResult>> ApplyValidationAndLogs(
        DateOnly referenceDate,
        EngineCallConfig engineCallConfig,
        List<WfActualDto>? wfActuals,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Validates prediction outputs based on collector wf engine outputs validation rules
    /// As part of DDD oriented method, we keep validation rules are intrinsic to wf output models
    /// This method is a just an application oriented wrapper around the validation logic for serving a business use case.
    /// </summary>
    /// <param name="referenceDate">requested engine calculation date</param>
    /// <param name="perCityEngineResponses">response to validate per city basis</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task<List<ValidationResult>> ApplyValidationAndLogs(
        DateOnly referenceDate, 
        IDictionary<int, WfEngineOutput> perCityEngineResponses, 
        CancellationToken cancellationToken);
    
}