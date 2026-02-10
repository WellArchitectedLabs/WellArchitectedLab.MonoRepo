using MasterData.Client.Dtos.Parameters;

namespace WeatherInsights.Collector.Domain.Ports.Config;

/// <summary>
/// Root config model for weather insights service
/// </summary>
public class WeatherInsightCollectorConfig
{
    public required EngineCallConfig EngineCallConfig { get; set; }
}

/// <summary>
/// This config is used for predictions engine calls
/// </summary>
public class EngineCallConfig
{
    /// <summary>
    /// We query master data service for this amount of years back for prediction
    /// </summary>
    public required int YearsHistoryDepth { get; set; }
    /// <summary>
    /// We instruct master data service to roll actual data by this provided configuration
    /// </summary>
    public required int RollingWindowDays { get; set; }
    /// <summary>
    /// For leap days (29th of february, we apply the following strategy)
    /// </summary>
    public required LeapDayResolutionStrategy LeapDayStrategy { get; set; }
}