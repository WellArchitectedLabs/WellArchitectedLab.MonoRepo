using MasterData.Application.Services.Interfaces;
using MasterData.Client.Dtos.Responses.City.GetAll;
using MasterData.Domain.AggregateModel.Cities;
using Microsoft.AspNetCore.Mvc;

namespace MasterData.Api.Controllers;

/// <summary>
/// API Controller for <see cref="City"/> entities
/// </summary>
/// <param name="cityService"></param>
[ApiController]
public class CitiesController(ICityService cityService)
{
    /// <summary>
    /// Returns all cities' list
    /// The list does not need to be paginated since they will not exceed the maximum allowed in parameter
    /// </summary>
    /// <returns>ReadOnlyCollection for <see cref="CityDto"/> object</returns>
    [Route("api/v1/city")]
    [HttpGet]
    public async Task<IReadOnlyCollection<CityDto>> GetAllCities(CancellationToken cancellationToken)
    {
        var allCities = await cityService.GetAll(cancellationToken);
        return CityDtoFactory.CreateFromDomain(allCities);
    }
}