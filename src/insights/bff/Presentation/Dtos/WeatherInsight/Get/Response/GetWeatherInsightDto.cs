namespace WeatherInsights.Bff.Api.Dtos.WeatherInsight.Get.Response;

/// <summary>
/// Dto related to Get insights API method
/// </summary>
public record GetWeatherInsightDto
{
    /// <summary>
    /// Timestamp associated to the actual value
    /// Insights are calculated on hourly basis for every day and every city
    /// For uniform calculation, the timestamp is stored in Utc
    /// </summary>
    public required DateTime TimestampUtc { get; init; }
    
    /// <summary>
    /// Celsius based temperature
    /// </summary>
    public required decimal Temperature { get; init; }
    
    /// <summary>
    /// Serves as a parameter serving for the weather forecasting engine
    /// </summary>
    public required decimal WindSpeed { get; init; }
    
    /// <summary>
    /// Serves as a parameter serving for the weather forecasting engine
    /// wiki: https://en.wikipedia.org/wiki/Precipitation
    /// </summary>
    public required decimal Precipitation { get; init; }
}

/// <summary>
/// Factory for creating <see cref="GetWeatherInsightDto"/> objects
/// </summary>
public static class GetWeatherInsightDtoFactory
{
    public static IEnumerable<GetWeatherInsightDto> CreateFromDomain(
        IEnumerable<Domain.AggregateModel.Insights.WeatherInsight> weatherInsights)
    {
        return weatherInsights.Select(domainInsights => new GetWeatherInsightDto
        {
            TimestampUtc = domainInsights.TimestampUtc,
            Temperature = domainInsights.Temperature,
            WindSpeed = domainInsights.WindSpeed,
            Precipitation = domainInsights.Precipitation
        });
    }
}