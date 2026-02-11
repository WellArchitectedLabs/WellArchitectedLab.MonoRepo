using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;

namespace WeatherInsights.Collector.Application.Factories;

/// <summary>
/// Factory methods around <see cref="WfInsight"/> entity
/// </summary>
public static class WfInsightFactory
{
    /// <summary>
    /// Converts prediction responses to insights, that can be stored into database
    /// </summary>
    /// <param name="perCityEngineOutput">prediction engine output for a single city</param>
    /// <returns></returns>
    public static IEnumerable<WfInsight> CreateFromEngineResponse(WfEngineOutput perCityEngineOutput)
     => perCityEngineOutput.PerHourPrediction.Select(perHourPredictionKeyValue => new WfInsight
        {
            CityId = perCityEngineOutput.CityId,
            TimestampUtc = perHourPredictionKeyValue.Key,
            Temperature = perHourPredictionKeyValue.Value.Temperature,
            WindSpeed = perHourPredictionKeyValue.Value.WindSpeed,
            Precipitation = perHourPredictionKeyValue.Value.Precipitation
        });
}