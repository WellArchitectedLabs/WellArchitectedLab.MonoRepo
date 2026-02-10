using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;

namespace WeatherInsights.Collector.Application.Factories;

/// <summary>
/// Factory methods around <see cref="WfInsightFactory"/>
/// </summary>
public static class WfInsightFactory
{
    /// <summary>
    /// Creates a list of <see cref="WfInsight"/> objects from an IEnumerable of <see cref="WfActualDto"/>
    /// </summary>
    /// <param name="wfActuals"></param>
    /// <returns></returns>
    public static IEnumerable<WfInsight> CreateFromWfActuals(IEnumerable<WfActualDto> wfActuals)
        => wfActuals.Select(wfActual => new WfInsight
            {
                CityId = wfActual.CityId,
                Precipitation = wfActual.Precipitation,
                Temperature = wfActual.Temperature,
                WindSpeed = wfActual.WindSpeed,
                TimestampUtc =  wfActual.TimestampUtc,
            });
    
}