using MasterData.Client.Dtos.Responses.WfActual.Get.History;

namespace WeatherInsights.Collector.Application.Extensions;

/// <summary>
/// Utiity methods around <see cref="WfActualDto"/>
/// </summary>
public static class WfActualExtensions
{
    /// <summary>
    /// Gets actual timestamp in <see cref="DateOnly"/> format
    /// </summary>
    /// <param name="wfActualDto"></param>
    public static DateOnly GetDateOnlyTimeStamp(this WfActualDto wfActualDto) =>
        DateOnly.FromDateTime(wfActualDto.TimestampUtc);
}