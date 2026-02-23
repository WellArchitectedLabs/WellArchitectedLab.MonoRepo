namespace MasterData.Api.Constants;

/// <summary>
/// Constants for wf insights collector API
/// </summary>
public static class ApiConstants
{
    /// <summary>
    /// Used in order to identify checks that would be integrated in readiness endpoint
    /// </summary>
    public const string ReadinessHealthCheckTag =  "readiness";
    
    /// <summary>
    /// We expose liveness endpoint on this uri
    /// </summary>
    public const string LivenessEndpoint = "/ping";
    
    /// <summary>
    /// We expose readiness endpoint on this uri
    /// </summary>
    public const string ReadinessEndpoint = "/ready";
    
    /// <summary>
    /// Default policy, non-restrictive for cross domain calls
    /// </summary>
    public const string DefaultCorsPolicy = "AllowAllPolicy";
}