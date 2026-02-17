using WeatherInsights.Collector.Client.Dtos;
using WeatherInsights.Collector.Client.Dtos.Responses.Get;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;

namespace WfInsights.Collector.Api.Factories;

/// <summary>
/// Factory around <see cref="GetWfInsightDto"/>
/// </summary>
public static class WfInsightDtoFactory
{
    /// <summary>
    /// Create a read only list of <see cref="GetWfInsightDto"/>
    /// From a domain list of domain <see cref="wfInsights"/>
    /// </summary>
    /// <param name="wfInsights">Read only list of <see cref="WfInsight"/></param>
    /// <returns></returns>
    public static IReadOnlyCollection<GetWfInsightDto> Create(List<WfInsight> wfInsights)
        => wfInsights.Select(wfInsight => new GetWfInsightDto
        {
            CityId = wfInsight.CityId,
            Id = wfInsight.Id,
            Precipitation = wfInsight.Precipitation,
            Temperature = wfInsight.Temperature,
            TimestampUtc = wfInsight.TimestampUtc,
            WindSpeed = wfInsight.WindSpeed
        }).ToList();
}