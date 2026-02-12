using Microsoft.AspNetCore.Mvc;
using WeatherInsights.Collector.Application.Interfaces;
using WeatherInsights.Collector.Client.Dtos.Parameters.Get;
using WfInsights.Collector.Api.Factories;

namespace WfInsights.Collector.Api.Controllers;

[ApiController]
public class WfInsightController(IWfInsightPipeline wfInsightPipeline) : Controller
{
    /// <summary>
    /// Kicks a prediction for the provided reference date
    /// Narrows down all cities from master database and sequentially calls prediction for each city
    /// The prediction result is then stored into database for later get operations.
    /// An audit is also kept for all prediction input / output history
    /// </summary>
    /// <param name="referenceDate">forecasting calculation date</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [Route("/api/wfInsight/{referenceDate}")]
    [HttpPost]
    public async Task<ActionResult> Post(
        [FromRoute] DateOnly referenceDate,
        CancellationToken cancellationToken)
    {
        await wfInsightPipeline.LaunchPrediction(referenceDate, cancellationToken);
        return Ok("Prediction was successful");
    }

    /// <summary>
    /// Get insights belonging to the given time range.
    /// </summary>
    /// <param name="cityId">City concerned by calculation</param>
    /// <param name="weatherInsightParameters">Get insights params object.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [Route("/api/wfInsight/{cityId}")]
    [HttpGet]
    public async Task<ActionResult> Get(
        [FromRoute] int cityId,
        [FromQuery] GetWeatherInsightParameters weatherInsightParameters,
        CancellationToken cancellationToken)
    {
        var domainInsights = await wfInsightPipeline.GetInsights(
            cityId,
            weatherInsightParameters.FromDateTime,
            weatherInsightParameters.ToDateTime,
            cancellationToken);
        return Ok(WfInsightDtoFactory.Create(domainInsights));
    } 
    
    
}