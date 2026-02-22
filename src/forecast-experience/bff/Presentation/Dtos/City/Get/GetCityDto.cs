namespace WfExperience.Bff.Api.Dtos.City.Get;

/// <summary>
/// City dto response from Get All cities endpoint
/// </summary>
public record GetCityDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}

/// <summary>
/// Factory for creating <see cref="GetCityDto"/> objects
/// </summary>
public static class GetCityDtoFactory
{
    public static IReadOnlyCollection<GetCityDto> CreateFromDomain(
        IEnumerable<WfExperience.Bff.Domain.AggregateModel.City.City> cities)
    {
        return cities.Select(domainCity => new GetCityDto
        {
            Id = domainCity.Id,
            Name = domainCity.Name
        }).ToList();
    }
}