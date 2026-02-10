using WeatherInsights.Collector.Application.Services;
using WeatherInsights.Collector.Application.Services.Interfaces;
using MasterData.Api.Extensions;
using MasterData.Client.Extensions;

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
        services.AddScoped<IWeatherInsightCollector, WeatherInsightCollector>();
        return services;
    }

    private static IServiceCollection ResgiterRepositories(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection RegisterHttpClients(this IServiceCollection services)
    {
        services.RegisterMasterDataClient("http://localhost:5000");
        return services;
    }
}