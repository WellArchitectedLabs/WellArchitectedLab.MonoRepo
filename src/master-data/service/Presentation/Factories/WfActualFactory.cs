using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using MasterData.Domain.AggregateModel.Actuals;

namespace MasterData.Api.Factories;

/// <summary>
/// Factory of <see cref="WfActual"/> type
/// </summary>
public static class WfActualDtoFactory
{
    public static IEnumerable<WfActualDto> CreateFromDomain(
        IEnumerable<WfActual> domainActuals)
        => domainActuals.Select(domainActual => new WfActualDto
        {
            TimestampUtc = domainActual.TimestampUtc,
            Temperature = domainActual.Temperature,
            WindSpeed = domainActual.WindSpeed,
            Precipitation = domainActual.Precipitation,
            CityId = domainActual.CityId
        });
}