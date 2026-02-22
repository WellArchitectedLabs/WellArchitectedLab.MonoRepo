using MasterData.Client.Dtos.Responses.WfActual.Get.History;

namespace WeatherInsights.Collector.Domain.Ports.HttpClients.Models;

/// <summary>
/// WF engine expects the following object for a single calculation request
/// </summary>
/// <param name="CityId">Needed by engine in order to identify the city concerned by calculation</param>
/// <param name="ReferenceDate">The date requested for calculation by callers (probably the calculation cron jobs)</param>
/// <param name="RequestTime">Time when insights service sent request to the prediction engine</param>
/// <param name="FarHistory">X Years wide histrory as requested by caller.
/// Counts at most 7 (max rolling days) * 2 (max past years) * 24 (number of hours in a day) = 336 actuals.
/// The calculation is made on single city basis.</param>
/// <param name="NearHistory">Near history. At max 7 days before the reference date.
/// Counts at most 7 (max number of rolling days) * 24 (number of hours per day) rows = 168 actuals </param>
public record WfEngineInput(
    int CityId,
    DateOnly ReferenceDate,
    DateTime RequestTime,
    IDictionary<DateTime, WfEngineInputActualItem> FarHistory,
    IDictionary<DateTime, WfEngineInputActualItem> NearHistory);

/// <summary>
/// Wf single actual for a given historical date
/// </summary>
/// <param name="Temperature">date temperature</param>
/// <param name="WindSpeed">date wind speeds</param>
/// <param name="Precipitation">date precipitation</param>
public record WfEngineInputActualItem(
    decimal Temperature,
    decimal WindSpeed,
    decimal Precipitation
)
{
    public WfEngineInputActualItem(GetHistoricalSlicesDto wfActualDto)  : this(wfActualDto.Temperature, wfActualDto.WindSpeed,
        wfActualDto.Precipitation)
    {}
}