namespace WeatherInsights.Collector.Domain.AggregateModel.Insight;

/// <summary>
/// A Wf insight is a weather calculation that is returned by the weather forecasting engine
/// Weather Collector arranges the needed input data for the weather forecasting engine to run.
/// Collector also stores the input payload for reproducible forecasts.
/// </summary>
public class WfInsight
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
    public required int CityId  { get; init; }
}