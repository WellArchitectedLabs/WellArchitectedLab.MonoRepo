using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using Microsoft.Extensions.Logging;
using WeatherInsights.Collector.Application.Extensions;
using WeatherInsights.Collector.Application.Interfaces;
using WeatherInsights.Collector.Domain.AggregateModel.Technical;
using WeatherInsights.Collector.Domain.AggregateModel.Technical.Visitors;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;
using WeatherInsights.Collector.Domain.Ports.Config;

namespace WeatherInsights.Collector.Application;

/// <summary>
/// Validates external data and logs errors when needed
/// This service is necessary for engine observability
/// </summary>
public class WfInsightMonitor(ILogger<WfInsightMonitor> logger) : IWfInsightMonitor
{
    private const string InternalServerErrorMessage =
        "An internal data validation error prevents engine call.";

    /// <inheritdoc/>
    public Task<List<ValidationResult>> ApplyValidationAndLogs(
        DateOnly referenceDate,
        MasterDataApiParameters masterDataApiParameters,
        List<WfActualDto>? wfActuals,
        CancellationToken cancellationToken)
    {
        if (wfActuals is null || !wfActuals.Any())
            throw new ApplicationException($"{InternalServerErrorMessage}.  Weather Forecast actuals are empty.");
        
        var validationResults = wfActuals.Validate(referenceDate, masterDataApiParameters);
        logger.LogValidationResults(validationResults);

        if (validationResults.Any(validation => validation.HasErrors))
            throw new ApplicationException($"{InternalServerErrorMessage}. Validation result: {string.Join(',', validationResults)}");
        
        return Task.FromResult(validationResults);
    }

    /// <inheritdoc/>
    public Task<List<ValidationResult>> ApplyValidationAndLogs(DateOnly referenceDate, IDictionary<int, WfEngineOutput> perCityEngineResponses, CancellationToken cancellationToken)
    {
        var validationResults = new List<ValidationResult>();
        foreach (var cityId in perCityEngineResponses.Keys)
        {
            var predictionOutput = perCityEngineResponses[cityId];
            validationResults.AddRange((predictionOutput.Validate(referenceDate)));
        }
        
        logger.LogValidationResults(validationResults);

        if (validationResults.Any(validation => validation.HasErrors))
            throw new ApplicationException($"{InternalServerErrorMessage}. Validation result: {string.Join(',', validationResults)}");
        return Task.FromResult(validationResults);
    }
}