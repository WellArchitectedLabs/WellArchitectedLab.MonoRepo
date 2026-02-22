using FluentValidation;
using MasterData.Client.Extensions;
using WeatherInsights.Bff.Api.FluentValidations;
using WeatherInsights.Bff.Application.Services;
using WeatherInsights.Bff.Application.Services.Interfaces;
using WeatherInsights.Bff.Domain.Ports.Adapters;
using WeatherInsights.Bff.Domain.Ports.Configuration;
using WeatherInsights.Bff.Infrastructure.Adapters;
using WeatherInsights.Collector.Client.Extensions;

namespace WeatherInsights.Bff.Api.Extensions;

/// <summary>
/// Extensions around <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterLayers(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.RegisterInfrastructureLayer(configuration)
            .RegisterApplicationLayer()
            .RegisterPresentationLayer();
    }

    private static IServiceCollection RegisterInfrastructureLayer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var insightsCollectorUrl = configuration.GetSection(
            StaticConfigurationPaths.InsightsCollectorUrlPath).Value;
        ArgumentException.ThrowIfNullOrWhiteSpace(insightsCollectorUrl);
        
        var masterDataClientUrl = configuration.GetSection(
            StaticConfigurationPaths.MasterDataUrlPath).Value;
        ArgumentException.ThrowIfNullOrWhiteSpace(masterDataClientUrl);
        
        services.RegisterInsightsCollectorClient(insightsCollectorUrl);
        services.RegisterMasterDataClient(masterDataClientUrl);
        
        services
            .AddScoped<IWeatherInsightsCollectorAdapter, HttpWeatherInsightsCollectorAdapter>()
            .AddScoped<IMasterDataAdapter, HttpMasterDataAdapter>();
        return services;
    }
    
    private static IServiceCollection RegisterApplicationLayer(this IServiceCollection services)
    {
        return services
            .AddScoped<IWeatherForecastService, WeatherForecastService>()
            .AddScoped<ICityService, CityService>();
    }
    
    private static IServiceCollection RegisterPresentationLayer(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining<GetWeatherInsightParameterValidation>();
    }
}