namespace WeatherInsights.Bff.Domain.Ports.Configuration;

/// <summary>
/// Before config registration and application builds and runs, we would like to access some configurations
/// With directly accessing them using the IConfiguration interface from Microsoft extensions abstractions.
/// This mechanism is only possible with storing constants representing these config paths
/// </summary>
public static class StaticConfigurationPaths
{
    public const string InsightsUrlPath = "InsightsCollector:Url";
    public const string MasterDataUrlPath = "MasterData:Url";
    public const string EndpointsConfig = "Endpoints";
}