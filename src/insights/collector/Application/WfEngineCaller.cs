using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using WeatherInsights.Collector.Application.Interfaces;
using WeatherInsights.Collector.Domain.Ports.HttpClients.Interfaces;
using WeatherInsights.Collector.Domain.Ports.HttpClients.Models;

namespace WeatherInsights.Collector.Application;

/// <summary>
/// Mediator class communicating with the weather insights forecasting engine
/// </summary>
/// <param name="wfEngineClient"></param>
/// <param name="logger"></param>
public class WfEngineCaller(
    IWfEngineClient wfEngineClient,
    ILogger<IWfEngineCaller> logger) : IWfEngineCaller
{
    /// <inheritdoc/>
    public async Task<IDictionary<int, WfEngineOutput>> CallEngine(
        IDictionary<int, WfEngineInput> perCityEngineInputs, 
        CancellationToken cancellationToken)
    {
        var cityIds = perCityEngineInputs.Keys.ToArray();
        var perCityEngineResponses = new Dictionary<int, WfEngineOutput>();

        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn(),
                new SpinnerColumn())
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Calling prediction engine", maxValue: cityIds.Length);
                foreach (var cityId in cityIds)
                {
                    // it is meant to be sequential in order to do not overload prediction engine with sudden load
                    // causing potentially a thundering herd problem and also an http port exhaustion if many calls are invoked
                    // please check: https://en.wikipedia.org/wiki/Thundering_herd_problem
                    try
                    {
                        var stopwatch = Stopwatch.StartNew();
                        var predictionResult =
                            await wfEngineClient.Call(perCityEngineInputs[cityId], cancellationToken);
                        stopwatch.Stop();
                        perCityEngineResponses.Add(cityId, predictionResult);
                        logger.LogInformation(
                            "City {CityId} completed in {ElapsedMs}ms",
                            cityId,
                            stopwatch.ElapsedMilliseconds);
                    }
                    catch (HttpRequestException httpRequestException)
                    {
                        logger.LogError(httpRequestException, httpRequestException.Message);
                    }
                    finally
                    {
                        task.Increment(1);
                    }
                }
            });

        return perCityEngineResponses;
    }
}