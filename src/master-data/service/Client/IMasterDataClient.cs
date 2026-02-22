using MasterData.Client.Dtos;
using MasterData.Client.Dtos.Parameters;
using MasterData.Client.Dtos.Responses.City.GetAll;
using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using Refit;

namespace MasterData.Client;

/// <summary>
/// Master data client that would be used to connect to master data API
/// As part of a bring your own client approach, this client will be published
/// as a nuget package for consumption by consuming teams
/// </summary>
public interface IMasterDataClient
{
    /// <summary>
    /// Returns all cities stored in master data database
    /// </summary>
    /// <returns></returns>
    [Get("/api/v1/city")]
    Task<IReadOnlyCollection<GetAllCitiesDto>> GetAllCities();

    /// <summary>
    /// Returns historical weather forecast details based on the provided history search parameters
    /// </summary>
    /// <param name="referenceDate">date subject of forecasting</param>
    /// <param name="historySearchParams"><see cref="HistorySearchParams"/></param>
    /// <param name="cancellationToken">provide cancellation token for stopping canceled processes down to downstream calls</param>
    /// <returns></returns>
    [Get("/api/v1/actual/{referenceDate}")] 
    Task<IReadOnlyCollection<GetHistoricalSlicesDto>> GetHistoricalSlices(
        DateOnly referenceDate,
        [Query] HistorySearchParams  historySearchParams,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Returns historical weather forecast details based on the provided history search parameters
    /// </summary>
    /// <param name="cityId">City concerned by actuals search.</param>
    /// <param name="from">We return actuals that are superior to this date time.</param>
    /// <param name="to">We return actuals that are inferior to this date time boundary.</param>
    /// <param name="cancellationToken">provide cancellation token for stopping canceled processes down to downstream calls</param>
    /// <remarks>Service is strict regarding the time part in the provided from / to date times.
    /// So please provide date times with the intended minutes / seconds.
    /// Is the time part is not relevant for you,
    /// then just provide a hour based time part with 00 minutes and 00 seconds</remarks>
    /// <returns></returns>
    [Get("/api/v1/actual")] 
    Task<IReadOnlyCollection<GetHistoricalSlicesDto>> GetByDateange(
        [Query] int cityId,
        [Query] DateTime from,
        [Query] DateTime  to,
        CancellationToken cancellationToken);
}