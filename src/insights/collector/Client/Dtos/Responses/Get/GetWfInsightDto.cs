namespace WeatherInsights.Collector.Client.Dtos.Responses.Get;

/// <summary>
/// Response model for GET /wfInsights
/// </summary>
public record GetWfInsightDto
{
    /// <summary>
    /// Auto-incremented Id
    /// </summary>
    public int Id { get; init; }

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

    /// <summary>
    /// Got from master data Db
    /// </summary>
    public required int CityId { get; init; }
}