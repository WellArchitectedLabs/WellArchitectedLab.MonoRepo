using MasterData.Client.Extensions;
using WeatherInsights.Collector.Application;
using WeatherInsights.Collector.Application.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Clients.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Config;
using WeatherInsights.Collector.Domain.Ports.Repositories.Interfaces;
using WeatherInsights.Collector.Infrastructure.Clients;
using WeatherInsights.Collector.Infrastructure.Connectors;
using WeatherInsights.Collector.Infrastructure.Repositories;

namespace WfInsights.Collector.Api.Extensions;

/// <summary>
/// Extensions around <see cref="IServiceCollection"/>
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all service collection layers
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration">necessary for accessing configs on runtime level</param>
    /// <returns></returns>
    internal static IServiceCollection RegisterLayers(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .ResgiterInfrastructure()
            .RegisterApplicationLayer(configuration);
    }
    
    /// <summary>
    /// Application Layer represents the classes and interfaces implementing user requirements
    /// They are high level and business oriented. They do not integrate adapter external specific logic
    /// like Data Access which will be part of the infrastructure registration method
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration">helps accessing runtime configuration on startup</param>
    /// <returns></returns>
    private static IServiceCollection RegisterApplicationLayer(this IServiceCollection services,  IConfiguration configuration)
    {
        services.AddScoped<IWfEngineInputCollector, WfEngineInputCollector>();
        services.AddScoped<IWfEngineCaller, WfEngineCaller>();
        services.AddScoped<IWfInsightModelPersister, WfInsightModelPersister>();
        services.AddScoped<IWfInsightMonitor, WfInsightMonitor>();
        services.AddScoped<IWfInsightPipeline, WfInsightPipeline>();
        return services.RegisterHttpClients(configuration);
    }
    
    /// <summary>
    /// Registers Infrastructure layer
    /// The separation between the two layers (application and infra) is very important as part
    /// of the current architectural choice which consists of a hexagonal architecture
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns></returns>
    private static IServiceCollection ResgiterInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IWfInsightRepository, WfInsightPgDbRepository>();
        services.AddScoped<IWfInsightAuditRepository, WfInsightPgDbAuditRepository>();
        services.AddScoped<IPostgresDbConnectionFactory, PostgresDbConnectionFactory>();
        return services;
    }

    private static IServiceCollection RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var masterDataServiceUrl = configuration.GetSection(StaticConfigurationPaths.MasterDataServiceUrlPath)?.Value;
        var wfEngineUrl = configuration.GetSection(StaticConfigurationPaths.WfEngineUrlPath)?.Value;
        ArgumentNullException.ThrowIfNull(masterDataServiceUrl);
        ArgumentNullException.ThrowIfNull(wfEngineUrl);
        services.RegisterMasterDataClient(masterDataServiceUrl);
        services.AddHttpClient<IWfEngineClient, WfEngineClient>(options => options.BaseAddress = new Uri(wfEngineUrl));
        return services;
    }
}