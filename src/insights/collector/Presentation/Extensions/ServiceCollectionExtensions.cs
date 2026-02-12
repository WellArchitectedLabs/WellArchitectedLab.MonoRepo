using WeatherInsights.Collector.Application.Services;
using WeatherInsights.Collector.Application.Services.Interfaces;
using MasterData.Api.Extensions;
using MasterData.Client.Extensions;
using WeatherInsights.Collector.Domain.Ports.Clients.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Repositories;
using WeatherInsights.Collector.Infrastructure.Clients;
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
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection RegisterLayers(this IServiceCollection services)
    {
        return services
            .RegisterHttpClients()
            .ResgiterRepositories()
            .RegisterServices();
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IWfEngineInputCollector, WfEngineInputCollector>();
        services.AddScoped<IWfEngineCaller, WfEngineCaller>();
        services.AddScoped<IWfInsightModelPersister, WfInsightModelPersister>();
        services.AddScoped<IWfInsightMonitor, WfInsightMonitor>();
        services.AddScoped<IWfInsightPipeline, WfInsightPipeline>();
        return services;
    }

    private static IServiceCollection ResgiterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWfInsightDbRepository, WfInsightRepository>();
        services.AddScoped<IWfInsightAuditRepository, WfInsightAuditRepository>();
        return services;
    }

    private static IServiceCollection RegisterHttpClients(this IServiceCollection services)
    {
        services.RegisterMasterDataClient("http://localhost:5000");
        services.AddHttpClient<IWfEngineClient, WfEngineClient>(options => options.BaseAddress = new Uri("http://localhost:5000"));
        return services;
    }
}