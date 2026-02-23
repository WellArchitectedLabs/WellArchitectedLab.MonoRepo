namespace WfExperience.Bff.Api.Constants;

/// <summary>
/// Constants for wf insights collector API
/// </summary>
public static class ApiConstants
{
    /// <summary>
    /// We expose liveness endpoint on this uri
    /// </summary>
    public const string LivenessEndpoint = "/ping";
    
    /// <summary>
    /// Allow all policy that is non-restrictive to cross domain calls
    /// So less secure than a valid cors policy
    /// </summary>
    public const string AllowAllPolicyName = "AllowAllPolicy";
}