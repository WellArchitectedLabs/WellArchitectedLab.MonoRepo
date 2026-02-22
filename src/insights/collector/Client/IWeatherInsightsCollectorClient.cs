using Refit;
using WeatherInsights.Collector.Client.Dtos.Parameters.Get;
using WeatherInsights.Collector.Client.Dtos.Responses.Get;

namespace WeatherInsights.Collector.Client;

/// <summary>
/// Master data client that would be used to connect to master data API
/// As part of a bring your own client approach, this client will be published
/// as a nuget package for consumption by consuming teams
/// </summary>
public interface IWeatherInsightsClient
{
    /// <summary>
    /// Returns all cities stored in master data database.
    /// </summary>
    /// <param name="referenceDate">Calculation date</param>
    /// <param name="cancellationToken">Please provide for proper cancellation management</param>
    /// <returns></returns>
    [Post("/api/v1/wfInsight/{referenceDate}")]
    Task<Task> LaunchPrediction(DateOnly referenceDate, CancellationToken cancellationToken);
    
    /// <summary>
    /// Returns historical weather forecast details based on the provided history search parameters.
    /// </summary>
    /// <param name="cityId">City which is subject of forecasting. Please query from Master Data API
    /// which is the golden source of cities and actuals.</param>
    /// <param name="weatherInsightParameters">Encapsulates weather insights parameters.</param>
    /// <param name="cancellationToken">Please provide for proper cancellation management.</param>
    /// <returns></returns>
    [Get("/api/v1/wfInsight/{cityId}")]
    Task<IReadOnlyCollection<GetWfInsightDto>> Get(
        int cityId,
        [Query] GetWeatherInsightParameters weatherInsightParameters,
        CancellationToken cancellationToken);
}