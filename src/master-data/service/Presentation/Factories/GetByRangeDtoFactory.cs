using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using MasterData.Client.Dtos.Responses.WfActual.Get.Range;
using MasterData.Domain.AggregateModel.Actuals;

namespace MasterData.Api.Factories;

public static class GetByRangeDtoFactory
{
    // <summary>
    /// Creates a list of <see cref="GetHistoricalSlicesDto"/> from an IEnummerable of domain objects
    /// </summary>
    /// <param name="domainActuals"></param>
    /// <returns></returns>
    public static IReadOnlyCollection<GetByRangeDto> CreateFromDomain(
        IEnumerable<WfActual> domainActuals)
        => domainActuals.Select(domainActual => new GetByRangeDto
        {
            TimestampUtc = domainActual.TimestampUtc,
            Temperature = domainActual.Temperature,
            WindSpeed = domainActual.WindSpeed,
            Precipitation = domainActual.Precipitation,
            CityId = domainActual.CityId
        }).ToList();
}