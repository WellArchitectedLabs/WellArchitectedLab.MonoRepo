namespace WeatherInsights.Collector.Domain.Ports.Config;

/// <summary>
/// Before config registration and application builds and runs, we would like to access some configurations
/// With directly accessing them using the IConfiguration interface from Microsoft extensions abstractions.
/// This mechanism is only possible with storing constants representing these config paths
/// </summary>
public static class StaticConfigurationPaths
{
    public const string WfEngineUrlPath = "WfEngine:Url";
    public const string MasterDataServiceUrlPath = "MasterDataService:Url";
}