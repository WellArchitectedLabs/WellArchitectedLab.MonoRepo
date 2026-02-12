using System.ComponentModel.DataAnnotations;
using MasterData.Client.Dtos.Parameters;

namespace WeatherInsights.Collector.Domain.Ports.Config;

/// <summary>
/// Root config model for weather insights service
/// </summary>
public class WeatherInsightCollectorConfig
{
    [Required] 
    public MasterDataServiceConfig MasterDataService { get; init; } = null!;
    [Required]
    public WfEngineConfig  WfEngine { get; init; } = null!;
    [Required]
    public EndpointsConfig Endpoints { get; init; } = null!;
}

/// <summary>
/// Master data api config model
/// </summary>
public class MasterDataServiceConfig
{
    [Required, MinLength(1)] 
    public string Url { get; init; } = null!;
    [Required] 
    public TimeStampsThresholdsConfig Thresholds { get; init; } = null!;
    [Required]
    public MasterDataApiParameters Parameters { get; init; } = null!;
}


/// <summary>
/// Master data api config model
/// </summary>
public class WfEngineConfig
{
    [Required, MinLength(1)] 
    public string Url { get; init; } = null!;
    [Required]
    public TimeStampsThresholdsConfig Thresholds { get; init; } = null!;
}

public class TimeStampsThresholdsConfig
{
    [Required, Range(1, 100)]
    public int MaxToleratedMissingHoursPercentage { get; init; }
    [Required]
    public bool EnforceErrorOnUnrelatedTimeStamps { get; init; }
}

/// <summary>
/// This config is used for predictions engine calls
/// </summary>
public class MasterDataApiParameters
{
    /// <summary>
    /// We query master data service for this amount of years back for prediction
    /// </summary>
    [Required, Range(1, 2)]
    public int YearsHistoryDepth { get; init; }
    /// <summary>
    /// We instruct master data service to roll actual data by this provided configuration
    /// </summary>
    [Required, Range(1, 7)]
    public int RollingWindowDays { get; init; }
    /// <summary>
    /// For leap days (29th of february, we apply the following strategy)
    /// </summary>
    [Required]
    public LeapDayResolutionStrategy LeapDayStrategy { get; init; }
}

/// <summary>
/// Config object for the weather insights API http endpoints
/// </summary>
public class EndpointsConfig
{
    /// <summary>
    /// GET api/v1/insight config
    /// </summary>
    [Required]
    public GetInsightsEndpoint GetInsightsV1 { get; init; } = null!;
}

/// <summary>
/// Separate configuration for GET v1/insights endpoint
/// </summary>
public class GetInsightsEndpoint
{
    /// <summary>
    /// Maximum time span difference between from date time and to date time in request payload
    /// </summary>
    [Required, Range(1, 60)]
    public int MaxRequestDateRangeInDays { get; init; }
}