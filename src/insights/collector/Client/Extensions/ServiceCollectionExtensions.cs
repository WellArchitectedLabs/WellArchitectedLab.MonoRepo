using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace WeatherInsights.Collector.Client.Extensions;

/// <summary>
/// Extensions around <see cref="IServiceCollection"/>
/// Exposed for consumers for streamlining the master data client usage (registration, settings ect...)
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// This utility method helps you register <see cref="IWeatherInsightsCollectorClient"/>
    /// Into startup by a single extension method call.
    /// Using this method, you would be able to register the client without extra configuration effort.
    /// Only the API address is needed since it depends on calling code environment.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to extend</param>
    /// <param name="apiAddress">Discovered weather insights api address</param>
    /// <returns></returns>
    public static IServiceCollection RegisterInsightsCollectorClient(this IServiceCollection services, string apiAddress)
    {
        // nothing special for the moment
        var settings = new RefitSettings();
        services.AddRefitClient<IWeatherInsightsCollectorClient>(settings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiAddress));
        
        return services;
    }
}