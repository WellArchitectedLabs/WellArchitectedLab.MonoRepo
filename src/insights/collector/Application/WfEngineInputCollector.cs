using MasterData.Client;
using MasterData.Client.Dtos.Parameters;
using Microsoft.Extensions.Options;
using WeatherInsights.Collector.Application.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Config;
using WeatherInsights.Collector.Domain.Ports.HttpClients.Models;
using WeatherInsights.Collector.Domain.Ports.HttpClients.Models.Factories;

namespace WeatherInsights.Collector.Application;

/// <summary>
/// Implements <see cref="IWfEngineInputCollector"/>
/// </summary>
/// <param name="masterDataClient"><see cref="IMasterDataClient"/></param>
/// <param name="masterDataServiceConfig">snapshot of <see cref="MasterDataServiceConfig"/></param>
public class WfEngineInputCollector(
    IMasterDataClient masterDataClient,
    IOptionsSnapshot<MasterDataServiceConfig> masterDataServiceConfig,
    IWfInsightMonitor insightMonitor) : IWfEngineInputCollector
{
    /// <inheritdoc/>
    public async Task<IDictionary<int, WfEngineInput>> CollectPredictionInput(
        DateOnly referenceDate, 
        CancellationToken cancellationToken)
    {
        var engineCallConfig = masterDataServiceConfig.Value.Parameters;
        var wfActuals =  (await masterDataClient.GetHistoricalSlices(
            referenceDate,
            new HistorySearchParams(engineCallConfig.YearsHistoryDepth, engineCallConfig.RollingWindowDays, engineCallConfig.LeapDayStrategy), cancellationToken)).ToList();
        await insightMonitor.ApplyValidationAndLogs(referenceDate, engineCallConfig, wfActuals, cancellationToken);
        var cityIds = (await masterDataClient.GetAllCities()).Select(c => c.Id).ToHashSet();
        return WfEngineInputFactory.CreateFromWfActuals(
            referenceDate,
            cityIds, 
            engineCallConfig, 
            wfActuals).ToDictionary(kv => kv.CityId);
    }
}