namespace WeatherInsights.Collector.Client.Dtos.Parameters.Get;

/// <summary>
/// Compact version of GET insights endpoint.
/// Please fill this object with GET params.
/// This object will be converted internally by the connector to a query style param, interpretable by the downstream Insights collector API.
/// </summary>
public record GetWeatherInsightParameters
{
    /// <summary>
    /// Date time from which calculation will be returned
    /// </summary>
    public required DateTime FromDateTime { get; init; }
    
    /// <summary>
    /// Date time to which calculation will be returned 
    /// </summary>
    public required  DateTime ToDateTime { get; init; }
}