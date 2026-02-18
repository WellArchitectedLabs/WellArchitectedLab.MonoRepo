using Microsoft.AspNetCore.Mvc;
using WeatherInsights.Bff.Api.Dtos.WeatherInsight.Get.Parameter;
using WeatherInsights.Bff.Api.Dtos.WeatherInsight.Get.Response;
using WeatherInsights.Bff.Application.Services.Interfaces;

namespace WeatherInsights.Bff.Api.Controllers;

[ApiController]
[Route("insight")]
public class WeatherInsightsController(IWeatherInsightsService weatherInsightsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get(
        [FromRoute] GetWeatherInsightParameter  parameter,
        CancellationToken cancellationToken)
    {
        var filteredInsights = await weatherInsightsService.Get(
            parameter.CityId, 
            parameter.FromDate, 
            parameter.ToDate, 
            cancellationToken);
        
        if(!filteredInsights.Any())
            return NoContent();
        
        return Ok(GetWeatherInsightDtoFactory.CreateFromDomain(filteredInsights));
    }
}
