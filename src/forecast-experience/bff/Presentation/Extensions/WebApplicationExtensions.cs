using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WfExperience.Bff.Api.Constants;

namespace WfExperience.Bff.Api.Extensions;

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
}