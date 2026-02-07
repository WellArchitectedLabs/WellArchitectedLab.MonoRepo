namespace MasterData.Infrastructure.Config;

/// <summary>
/// Configuration entity class, read from app settings file
/// </summary>
public class MasterDataServiceConfig
{
    /// <summary>
    /// Threshold before logging warning on unsupported number of managed cities
    /// </summary>
    public required int MaximumCitiesThreshold { get; init; }
}