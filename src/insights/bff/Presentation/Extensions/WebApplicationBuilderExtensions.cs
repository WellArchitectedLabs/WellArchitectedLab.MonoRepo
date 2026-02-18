using WeatherInsights.Bff.Domain.Ports.Configuration;

namespace WeatherForecast.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void SetupConfiguration(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        webApplicationBuilder.Configuration.AddEnvironmentVariables();
        
        // validate insight service config and make it accessible via options pattern
        webApplicationBuilder.Services
            .AddOptions<WeatherInsightsCollectorConfig>()
            .Bind(webApplicationBuilder.Configuration.GetSection(StaticConfigurationPaths.InsightsCollector))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        // validate master data service config and make it accessible via options pattern
        webApplicationBuilder.Services
            .AddOptions<MasterDataConfig>()
            .Bind(webApplicationBuilder.Configuration.GetSection(StaticConfigurationPaths.MasterData))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        // validate endpoints configs and make them accessible via options pattern
        webApplicationBuilder.Services
            .AddOptions<WeatherInsightsBffEndpointConfig>()
            .Bind(webApplicationBuilder.Configuration.GetSection(StaticConfigurationPaths.EndpointsConfig))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
