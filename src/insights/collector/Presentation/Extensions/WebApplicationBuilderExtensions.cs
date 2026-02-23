using Microsoft.Extensions.Diagnostics.HealthChecks;
using WeatherInsights.Collector.Domain.Ports.Config;
using WfInsights.Collector.Api.Constants;

namespace WfInsights.Collector.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Setup configuration for later dependency injection
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    public static void SetupConfiguration(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        webApplicationBuilder.Configuration.AddEnvironmentVariables();
        
        webApplicationBuilder.Services
            .AddOptions<WfEngineConfig>()
            .Bind(webApplicationBuilder.Configuration.GetSection("WfEngine"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        webApplicationBuilder.Services
            .AddOptions<MasterDataServiceConfig>()
            .Bind(webApplicationBuilder.Configuration.GetSection("MasterDataService"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        webApplicationBuilder.Services
            .AddOptions<EndpointsConfig>()
            .Bind(webApplicationBuilder.Configuration.GetSection("Endpoints"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
    
    /// <summary>
    /// Sets up readiness checks
    /// </summary>
    /// <param name="builder"></param>
    public static void RegisterReadinessChecks(this WebApplicationBuilder builder)
    {
        var npgSqlConnectionString = builder.Configuration.GetConnectionString(
            StaticConfigurationPaths.NpgSqlConnectionString);
        
        builder.Services.AddHealthChecks()
            .AddNpgSql(
                npgSqlConnectionString!,
                name: "npgsql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["database", "postgres", ApiConstants.ReadinessHealthCheckTag]
            );
    }
}
