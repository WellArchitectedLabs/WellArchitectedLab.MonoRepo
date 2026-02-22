using MasterData.Client.Dtos.Responses.City.GetAll;

namespace MasterData.Api.Factories;

/// <summary>
/// Factory for <see cref="GetAllCitiesDto"/>
/// </summary>
public static class GetAllCitiesDtoFactory
{
    /// <summary>
    /// Creates a city dto read only list from the provided cities collection
    /// </summary>
    /// <param name="cities">list of domain cities</param>
    /// <returns></returns>
    public static IReadOnlyCollection<GetAllCitiesDto> 
        CreateFromDomain(IReadOnlyCollection<Domain.AggregateModel.Cities.City> cities)
        => cities.Select(
                c => new GetAllCitiesDto(c.Id, c.Name, GetAllCitiesGpsCoordinatesDtoFactory.CreateFromDomain(c.Coordinates)))
            .ToList();
}