using MasterData.Api.Factories;
using MasterData.Application.Services.Interfaces;
using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using MasterData.Client.Dtos.Responses.WfActual.Get.Range;
using MasterData.Domain.AggregateModel.Actuals;
using MasterData.Domain.AggregateModel.Actuals.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MasterData.Api.Controllers;

/// <summary>
/// API Controller for <see cref="WfActual"/> entities
/// </summary>
/// <param name="actualService">actuals service</param>
[ApiController]
[Route("api/v1/actual")]
public class WfActualController(IWfActualService actualService)
{
    /// <summary>
    /// Returns weather forecast history based on the provided search parameters
    /// </summary>
    /// <returns>ReadOnlyCollection for <see cref="GetHistoricalSlicesDto"/> object</returns>
    /// <param name="referenceDate">date subject of forecasting</param>
    /// <param name="historicalDepthYears">number of past years of history returned for prediction analysis</param>
    /// <param name="rollingWindowDays">odd number representing the number of days to pick in every historical year.</param>
    /// <param name="leapDayResolutionStrategy">resolving strategy for leap years (29th of February)</param>
    /// <param name="cancellationToken">provide cancellation token for stopping canceled processes down to downstream calls</param>
    [Route("{referenceDate}")]
    [HttpGet]
    public async Task<IReadOnlyCollection<GetHistoricalSlicesDto>> GetHistoricalSlices(
        [FromRoute] DateOnly referenceDate,
        [FromQuery] int historicalDepthYears,
        [FromQuery] int rollingWindowDays,
        [FromQuery] LeapDayResolutionStrategy leapDayResolutionStrategy,
        CancellationToken cancellationToken)
    {
        var historicalSlices = await actualService.GetHistoricalSlice(
            referenceDate,
            historicalDepthYears,
            rollingWindowDays,
            leapDayResolutionStrategy,
            cancellationToken);
        return GetHistoricalSlicesDtoFactory.CreateFromDomain(historicalSlices).ToList();
    }

    /// <summary>
    /// Returns weather forecast history in the given range
    /// </summary>
    /// <returns>ReadOnlyCollection for <see cref="GetHistoricalSlicesDto"/> object</returns>
    /// <param name="cityId">city Id</param>
    /// <param name="from">date subject of forecasting</param>
    /// <param name="to">number of past years of history returned for prediction analysis</param>
    /// <param name="ct">provide cancellation token for stopping canceled processes down to downstream calls</param>
    [HttpGet]
    public async Task<IReadOnlyCollection<GetByRangeDto>> Search(
        [FromQuery] int cityId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var getByRangeFromService = await actualService.Search(cityId, from, to, ct);
        return GetByRangeDtoFactory.CreateFromDomain(getByRangeFromService).ToList();
    }
}