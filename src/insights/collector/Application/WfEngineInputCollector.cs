using MasterData.Client;
using MasterData.Client.Dtos.Parameters;
using Microsoft.Extensions.Options;
using WeatherInsights.Collector.Application.Factories;
using WeatherInsights.Collector.Application.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;
using WeatherInsights.Collector.Domain.Ports.Config;

namespace WeatherInsights.Collector.Application;

/// <summary>
/// Implements <see cref="IWfEngineInputCollector"/>
/// </summary>
/// <param name="masterDataClient"><see cref="IMasterDataClient"/></param>
/// <param name="configSnapshot">snapshot of <see cref="WeatherInsightCollectorConfig"/></param>
public class WfEngineInputCollector(
    IMasterDataClient masterDataClient,
    IOptionsSnapshot<WeatherInsightCollectorConfig> configSnapshot,
    WfInsightMonitor insightMonitor) : IWfEngineInputCollector
{
    /// <inheritdoc/>
    public async Task<IDictionary<int, WfEngineInput>> CollectPredictionInput(
        DateOnly referenceDate, 
        CancellationToken cancellationToken)
    {
        var engineCallConfig = configSnapshot.Value.MasterDataApi.Parameters;
        var wfActuals =  (await masterDataClient.GetHistoricalSlices(
            referenceDate, 
            new HistorySearchParams(engineCallConfig.YearsHistoryDepth, engineCallConfig.RollingWindowDays, engineCallConfig.LeapDayStrategy), 
            cancellationToken)).ToList();
        await insightMonitor.ApplyValidationAndLogs(referenceDate, engineCallConfig, wfActuals, cancellationToken);
        var cityIds = (await masterDataClient.GetAllCities()).Select(c => c.Id).ToHashSet();
        return WfEngineInputFactory.CreateFromWfActuals(
            referenceDate,
            cityIds, 
            engineCallConfig, 
            wfActuals).ToDictionary(kv => kv.CityId);
    }
}