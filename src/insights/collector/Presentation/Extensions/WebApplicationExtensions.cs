using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WfInsights.Collector.Api.Constants;

namespace WfInsights.Collector.Api.Extensions;

/// <summary>
/// Extensions around <see cref="WebApplication"/>
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Exposes a /health endpoint for API liveness
    /// </summary>
    /// <param name="app">a built web application</param>
    public static void MapLiveness(this WebApplication app)
    {
        app.MapHealthChecks(ApiConstants.LivenessEndpoint, new HealthCheckOptions
        {
            AllowCachingResponses = false
        });
    }

    /// <summary>
    /// Exposes a /ready endpoint for API readiness
    /// </summary>
    /// <param name="app">a built web application</param>
    public static void MapReadiness(this WebApplication app)
    {
        app.MapHealthChecks(ApiConstants.ReadinessEndpoint, new HealthCheckOptions
        {
            AllowCachingResponses = false,
            Predicate = r => r.Tags.Contains(ApiConstants.ReadinessHealthCheckTag)
        });
    }
}