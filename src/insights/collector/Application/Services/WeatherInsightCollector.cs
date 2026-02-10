using MasterData.Client;
using MasterData.Client.Dtos.Parameters;
using Microsoft.Extensions.Options;
using WeatherInsights.Collector.Application.Factories;
using WeatherInsights.Collector.Application.Services.Interfaces;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;
using WeatherInsights.Collector.Domain.Ports.Config;
using WeatherInsights.Collector.Domain.Ports.Repositories;

namespace WeatherInsights.Collector.Application.Services;

public class WeatherInsightCollector(
    IMasterDataClient masterDataClient, 
    IOptionsSnapshot<WeatherInsightCollectorConfig> configSnapshot,
    IWfInsightDbRepository wfInsightDbRepository,
    IWfInsightAuditDbRepository wfInsightAuditDbRepository) : IWeatherInsightCollector
{
    public async Task CollectPrediction(DateOnly referenceDate, CancellationToken cancellationToken)
    {
        var cities = masterDataClient.GetAllCities();
        var engineCallConfig = configSnapshot.Value.EngineCallConfig;
        var wfActuals =  await masterDataClient.GetHistoricalSlices(
            referenceDate, 
            new HistorySearchParams(engineCallConfig.YearsHistoryDepth, engineCallConfig.RollingWindowDays, engineCallConfig.LeapDayStrategy), 
            cancellationToken);
        var wfInsights = WfInsightFactory.CreateFromWfActuals(wfActuals);
        await wfInsightDbRepository.Save(wfInsights, cancellationToken);
        await wfInsightAuditDbRepository.Save(wfInsights, cancellationToken);
    }
}