using MasterData.Client.Dtos.Responses.City.GetAll;
using MasterData.Domain.AggregateModel.Cities.ValueObjects;

namespace MasterData.Api.Factories;

/// <summary>
/// Factory for <see cref="GetAllCitiesGpsCoordinatesDto"/>
/// </summary>
public static class GetAllCitiesGpsCoordinatesDtoFactory
{
    /// <summary>
    /// Creates a dto object from a domain <see cref="GpsCoordinates"/> entity
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public static GetAllCitiesGpsCoordinatesDto
        CreateFromDomain(GpsCoordinates coordinate) =>
        new GetAllCitiesGpsCoordinatesDto(coordinate.Latitude, coordinate.Longitude);
}    