using MasterData.Api.Dtos.City.GetAll;
using MasterData.Api.Dtos.WfActual.Get.History;
using MasterData.Application.Services.Interfaces;
using MasterData.Domain.AggregateModel.Actuals;
using MasterData.Domain.AggregateModel.Actuals.Enums;
using MasterData.Domain.AggregateModel.Cities;
using Microsoft.AspNetCore.Mvc;

namespace MasterData.Api.Controllers;

/// <summary>
/// API Controller for <see cref="WfActual"/> entities
/// </summary>
/// <param name="actualsService"></param>
[ApiController]
public class WfActualsController(IWfActualService actualsService)
{
    /// <summary>
    /// Returns all cities' list
    /// The list does not need to be paginated since they will not exceed the maximum allowed in parameter
    /// </summary>
    /// <returns>ReadOnlyCollection for <see cref="CityDto"/> object</returns>
    [Route("api/v1/actuals/{referenceDate}")]
    [HttpGet]
    public async Task<IReadOnlyCollection<WfActualDto>> GetHistoricalSlices(
        [FromRoute] DateOnly referenceDate,
        [FromQuery] int historicalDepthYears,
        [FromQuery] int rollingWindowDays,
        [FromQuery] LeapDayResolutionStrategy leapDayResolutionStrategy,
        CancellationToken cancellationToken)
    {
        var historicalSlices = await actualsService.GetHistoricalSlice(
            referenceDate,
            historicalDepthYears,
            rollingWindowDays,
            leapDayResolutionStrategy,
            cancellationToken);
        return WfActualDtoFactory.CreateFromDomain(historicalSlices).ToList();
    }
}