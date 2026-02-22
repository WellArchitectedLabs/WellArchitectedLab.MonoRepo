using FluentValidation;
using MasterData.Client.Extensions;
using WeatherInsights.Collector.Client.Extensions;
using WfExperience.Bff.Api.FluentValidations;
using WfExperience.Bff.Application.Services;
using WfExperience.Bff.Application.Services.Interfaces;
using WfExperience.Bff.Domain.Ports.Adapters;
using WfExperience.Bff.Domain.Ports.Configuration;
using WfExperience.Bff.Infrastructure.Adapters;

namespace WfExperience.Bff.Api.Extensions;

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