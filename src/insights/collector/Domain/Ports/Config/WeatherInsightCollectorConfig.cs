using MasterData.Client.Dtos.Parameters;

namespace WeatherInsights.Collector.Domain.Ports.Config;

/// <summary>
/// Root config model for weather insights service
/// </summary>
public class WeatherInsightCollectorConfig
{
    public required MasterDataApiConfig  MasterDataApi { get; set; }
    public required WfEngineConfig  WfEngine { get; set; }
    public required EndpointsConfig Endpoints { get; set; }
}

/// <summary>
/// Master data api config model
/// </summary>
public class MasterDataApiConfig
{
    public required string Url { get; set; }
    public required MasterDataApiValidationThresholds Thresholds { get; set; }
    public required MasterDataApiParameters Parameters { get; set; }
}


/// <summary>
/// Master data api config model
/// </summary>
public class WfEngineConfig
{
    public required string Url { get; set; }
    public required WfEngineValidationThresholdsConfig Thresholds { get; set; }
}

public class MasterDataApiValidationThresholds
{
    public int MaxToleratedMissingHoursPercentage { get; set; }
    public int EnforceErrorOnUnrelatedTimeStamps { get; set; }
}

public class WfEngineValidationThresholdsConfig
{
    public int MaxToleratedMissingHoursPercentage { get; set; }
    public bool EnforceErrorOnUnrelatedTimeStamps { get; set; }
}

/// <summary>
/// This config is used for predictions engine calls
/// </summary>
public class MasterDataApiParameters
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

/// <summary>
/// Config object for the weather insights API http endpoints
/// </summary>
public class EndpointsConfig
{
    /// <summary>
    /// GET api/v1/insight config
    /// </summary>
    public required GetInsightsEndpoint GetInsightsV1 { get; set; }
}

/// <summary>
/// Separate configuration for GET v1/insights endpoint
/// </summary>
public class GetInsightsEndpoint
{
    /// <summary>
    /// Maximum time span difference between from date time and to date time in request payload
    /// </summary>
    public required int MaxRequestDateRangeInDays { get; set; }
}