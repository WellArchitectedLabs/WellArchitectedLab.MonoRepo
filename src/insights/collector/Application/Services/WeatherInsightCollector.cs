using WeatherInsights.Collector.Application.Services.Interfaces;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;

namespace WeatherInsights.Collector.Application.Services;

public class WeatherInsightCollector : IWeatherInsightCollector
{
    public Task<WfInsight> CollectData(DateTime timeStampUtc, int cityIds)
    {
        throw new NotImplementedException();
    }
}