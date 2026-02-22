using Microsoft.AspNetCore.Mvc;
using WfExperience.Bff.Api.Dtos.WeatherInsight.Get.Parameter;
using WfExperience.Bff.Api.Dtos.WeatherInsight.Get.Response;
using WfExperience.Bff.Application.Services.Interfaces;

namespace WfExperience.Bff.Api.Controllers;

[ApiController]
[Route("api/v1/forecast")]
public class WeatherForecastController(IWeatherForecastService weatherForecastService) : ControllerBase
{
    [HttpGet("{cityId}/{fromDate}/{toDate}")]
    public async Task<ActionResult> Get(
        [FromRoute] GetWeatherInsightParameter  parameter,
        CancellationToken cancellationToken)
    {
        var filteredInsights = await weatherForecastService.Get(
            parameter.CityId, 
            parameter.FromDate, 
            parameter.ToDate, 
            cancellationToken);
        
        if(!filteredInsights.Any())
            return NoContent();
        
        return Ok(GetWeatherInsightDtoFactory.CreateFromDomain(filteredInsights));
    }
}
