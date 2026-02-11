using MasterData.Client;
using MasterData.Client.Dtos.Parameters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherInsights.Collector.Application.Factories;
using WeatherInsights.Collector.Application.Services.Interfaces;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;
using WeatherInsights.Collector.Domain.Ports.Clients.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;
using WeatherInsights.Collector.Domain.Ports.Config;
using WeatherInsights.Collector.Domain.Ports.Repositories;

namespace WeatherInsights.Collector.Application.Services;

/// <summary>
/// Implements <see cref="IWeatherInsightCollector"/>
/// </summary>
/// <param name="masterDataClient"></param>
/// <param name="weatherInsightsEngine"></param>
/// <param name="configSnapshot"></param>
/// <param name="wfInsightDbRepository"></param>
/// <param name="wfInsightAuditDbRepository"></param>
/// <param name="logger">logs errors and important infos</param>
public class WeatherInsightCollector(
    IMasterDataClient masterDataClient,
    IWeatherInsightsEngine weatherInsightsEngine,
    IOptionsSnapshot<WeatherInsightCollectorConfig> configSnapshot,
    IWfInsightDbRepository wfInsightDbRepository,
    IWfInsightAuditDbRepository wfInsightAuditDbRepository,
    ILogger<WeatherInsightCollector> logger) : IWeatherInsightCollector
{
    public async Task CollectPrediction(
        DateOnly referenceDate, 
        CancellationToken cancellationToken)
    {
        var engineCallConfig = configSnapshot.Value.EngineCallConfig;
        var wfActuals =  await masterDataClient.GetHistoricalSlices(
            referenceDate, 
            new HistorySearchParams(engineCallConfig.YearsHistoryDepth, engineCallConfig.RollingWindowDays, engineCallConfig.LeapDayStrategy), 
            cancellationToken);
        // validate actuals before engine call (log warnings if necessary or stop execution for hard errors)
        var cityIds = (await masterDataClient.GetAllCities()).Select(c => c.Id).ToHashSet();
        var perCityEngineResponses = new Dictionary<int, WfEngineOutput>();
        var perCityEngineInputs = WfEngineInputFactory.CreateFromWfActuals(
            referenceDate,
            cityIds, 
            engineCallConfig, 
            wfActuals).ToDictionary(kv => kv.CityId);
        foreach (var cityId in cityIds)
        {
            // it is meant to be sequential in order to do not overload prediction engine with sudden load
            // causing potentially a thundering herd problem and also an http port exhaustion if many calls are invoked
            // please check: https://en.wikipedia.org/wiki/Thundering_herd_problem
            try
            {
                var predictionResult = await weatherInsightsEngine.Call(perCityEngineInputs[cityId], cancellationToken);
                perCityEngineResponses.Add(cityId, predictionResult);
            }
            catch (HttpRequestException httpRequestException)
            {
                logger.LogError(httpRequestException, httpRequestException.Message);
            }
        }
        // validate engine response (that hourly timestamps are returned ect...)
        // the validation are only surface level
        // calibration service will handle functional drifts
        LogEngineResponseAnomalies(referenceDate, perCityEngineResponses);
        
        var perCityInsights = await TransformAndSaveWfInsights(cancellationToken, perCityEngineResponses);
        await TransformAndSaveWfInsightAudits(cancellationToken, cityIds, perCityInsights, perCityEngineInputs, perCityEngineResponses);
    }

    #region private methods

    private async Task TransformAndSaveWfInsightAudits(CancellationToken cancellationToken, HashSet<int> cityIds,
        Dictionary<int, IEnumerable<WfInsight>> perCityInsights, Dictionary<int, WfEngineInput> perCityEngineInputs, Dictionary<int, WfEngineOutput> perCityEngineResponses)
    {
        var wfInsightAudits = cityIds.SelectMany(
            cityId => 
                // we project audit by concerned insights
                perCityInsights[cityId].Select(wfIns => WfInsightAuditFactory.CreateFromWfInsight(perCityEngineInputs[cityId], perCityEngineResponses[cityId], wfIns.Id)));
        await wfInsightAuditDbRepository.Save(wfInsightAudits, cancellationToken);
    }

    private async Task<Dictionary<int, IEnumerable<WfInsight>>> TransformAndSaveWfInsights(CancellationToken cancellationToken, Dictionary<int, WfEngineOutput> perCityEngineResponses)
    {
        var perCityInsights = perCityEngineResponses.ToDictionary(kv => kv.Key, kv => WfInsightFactory.CreateFromEngineResponse(kv.Value));
        await wfInsightDbRepository.Save(perCityInsights.Values.SelectMany(wIns => wIns), cancellationToken);
        return perCityInsights;
    }

    private void LogEngineResponseAnomalies(DateOnly referenceDate, Dictionary<int, WfEngineOutput> perCityEngineResponses)
    {
        foreach (var cityId in perCityEngineResponses.Keys)
        {
            var predictionOutput = perCityEngineResponses[cityId];
            predictionOutput.ValidateAndLog(referenceDate, cityId, logger);
        }
    }

    #endregion
}