using Microsoft.AspNetCore.Mvc;
using WeatherInsights.Bff.Api.Dtos.City.Get;
using WeatherInsights.Bff.Api.Dtos.WeatherInsight.Get.Parameter;
using WeatherInsights.Bff.Application.Services.Interfaces;

namespace WeatherInsights.Bff.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CityController(ICityService cityService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get(
        CancellationToken cancellationToken)
    {
        var allCities = await cityService.GetAll(cancellationToken);
        
        if(!allCities.Any())
            return NoContent();
        
        return Ok(GetCityDtoFactory.CreateFromDomain(allCities));
    }
}