using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using MasterData.Domain.AggregateModel.Actuals;

namespace MasterData.Api.Factories;

/// <summary>
/// Factory of <see cref="WfActual"/> type
/// </summary>
public static class GetHistoricalSlicesDtoFactory
{
    /// <summary>
    /// Creates a list of <see cref="GetHistoricalSlicesDto"/> from an ienum of domain objects
    /// </summary>
    /// <param name="domainActuals"></param>
    /// <returns></returns>
    public static IReadOnlyCollection<GetHistoricalSlicesDto> CreateFromDomain(
        IEnumerable<WfActual> domainActuals)
        => domainActuals.Select(domainActual => new GetHistoricalSlicesDto
        {
            TimestampUtc = domainActual.TimestampUtc,
            Temperature = domainActual.Temperature,
            WindSpeed = domainActual.WindSpeed,
            Precipitation = domainActual.Precipitation,
            CityId = domainActual.CityId
        }).ToList();
}