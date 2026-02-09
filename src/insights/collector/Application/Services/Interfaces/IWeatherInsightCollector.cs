namespace WeatherInsights.Collector.Application.Services.Interfaces;

using Domain.AggregateModel.Insight;

/// <summary>
/// 
/// </summary>
public interface IWeatherInsightCollector
{
    /// <summary>
    /// Aggregates a weather insight historical details from the system
    /// Then posts and weather insight payload to the forecasting engine
    /// </summary>
    /// <param name="timeStampUtc"></param>
    /// <param name="cityIds"></param>
    /// <returns></returns>
    Task<WfInsight> CollectData(DateTime timeStampUtc, int cityIds);
}