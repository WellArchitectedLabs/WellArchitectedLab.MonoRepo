namespace WeatherInsights.Bff.Domain.Ports.Configuration;

/// <summary>
/// Before config registration and application builds and runs, we would like to access some configurations
/// With directly accessing them using the IConfiguration interface from Microsoft extensions abstractions.
/// This mechanism is only possible with storing constants representing these config paths
/// </summary>
public static class StaticConfigurationPaths
{
    #region private constants

    private const string UrlSuffix = "Url";

    #endregion

    #region public constants
    
    
    public const string InsightsCollector = "InsightsCollector";
    public const string MasterData = "MasterData";
    public const string InsightsCollectorUrlPath = $"{InsightsCollector}:{UrlSuffix}";
    public const string MasterDataUrlPath = $"{MasterData}:{UrlSuffix}";
    public const string EndpointsConfig = "Endpoints";

    #endregion
}