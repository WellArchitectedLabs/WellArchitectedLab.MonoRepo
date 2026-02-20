using MasterData.Client.Dtos.Responses.City.GetAll;

namespace MasterData.Api.Factories;

/// <summary>
/// Factory for <see cref="CityDto"/>
/// </summary>
public static class CityDtoFactory
{
    /// <summary>
    /// Creates a city dto read only list from the provided cities collection
    /// </summary>
    /// <param name="cities">list of domain cities</param>
    /// <returns></returns>
    public static IReadOnlyCollection<CityDto> 
        CreateFromDomain(IReadOnlyCollection<Domain.AggregateModel.Cities.City> cities)
        => cities.Select(
                c => new CityDto(c.Id, c.Name, GpsCoordinatesDtoFactory.CreateFromDomain(c.Coordinates)))
            .ToList();
}