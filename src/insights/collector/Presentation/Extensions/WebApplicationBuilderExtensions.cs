using WeatherInsights.Collector.Domain.Ports.Config;

namespace WfInsights.Collector.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
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
}
