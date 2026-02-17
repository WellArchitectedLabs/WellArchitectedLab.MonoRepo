using System.ComponentModel.DataAnnotations;

namespace WeatherInsights.Bff.Domain.Ports.Configuration;

/// <summary>
/// Root Api Config
/// </summary>
public class WeatherInsightsBffConfig
{
    /// <summary>
    /// Insights collector related configs
    /// </summary>
    [Required]
    public WeatherInsightsCollectorConfig InsightsCollector { get; init; } = null!;
    
    /// <summary>
    /// Master data related configs
    /// </summary>
    [Required] 
    public MasterDataConfig MasterData { get; init; } = null!;
}

/// <summary>
/// Config definition related the insights collector
/// </summary>
public class WeatherInsightsCollectorConfig
{
    [Required, MinLength(1)]
    public string Url { get; init; } = null!;
}

/// <summary>
/// Config definition related the master data service
/// </summary>
public class MasterDataConfig
{
    [Required, MinLength(1)]
    public string Url { get; init; } = null!;
}

/// <summary>
/// Endpoints configs
/// </summary>
public class WeatherInsightsBffEndpointConfig
{
    [Required]
    public GetInsightV1Config GetInsightV1 { get; init; } = null!;
}

/// <summary>
/// Configs related to insight controller's GET insight endpoint
/// </summary>
public class GetInsightV1Config
{
    /// <summary>
    /// GET insights requests need to have to date field inferior to now + this configured number of days
    /// Set as unsigned int because it needs to be positive in order to force the limit to a future date
    /// </summary>
    [Required]
    public uint ToDateMaxDaysFromNow { get; init; }
}