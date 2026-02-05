using System.Data;
using MasterData.Application.Services;
using MasterData.Application.Services.Interfaces;
using MasterData.Domain.Ports;
using MasterData.Infrastructure.Connectors;
using MasterData.Infrastructure.Repositories;

namespace MasterData.Api.Extensions;

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
            .ResgiterRepositories()
            .RegisterServices();
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IWfActualService, WfActualService>();
        return services;
    }

    private static IServiceCollection ResgiterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWfActualRepository, WfActualPgDbRepository>();
        services.AddScoped<ICityRepository, CityPgDbRepository>();
        services.AddScoped<IPostgresDbConnectionFactory, PostgresDbConnectionFactory>();
        return services;
    }
}