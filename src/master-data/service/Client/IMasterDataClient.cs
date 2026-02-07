using MasterData.Api.Dtos.WfActual.Get.History;
using MasterData.Client.Dtos;
using MasterData.Client.Dtos.Parameters;
using MasterData.Client.Dtos.Responses.City.GetAll;
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
    [Get("api/v1/city")]
    Task<IEnumerable<CityDto>> GetAllCities();
    
    /// <summary>
    /// Returns historical weather forecast details based on the provided history search parameters
    /// </summary>
    /// <param name="referenceDate">date subject of forecasting</param>
    /// <param name="historySearchParams"><see cref="HistorySearchParams"/></param>
    /// <param name="cancellationToken">provide cancellation token for stopping canceled processes down to downstream calls</param>
    /// <returns></returns>
    [Get("api/v1/actual/{referenceDate}")]
    public Task<IEnumerable<WfActualDto>> GetHistoricalSlices(
        DateOnly referenceDate,
        [Query] HistorySearchParams  historySearchParams,
        CancellationToken cancellationToken);
}