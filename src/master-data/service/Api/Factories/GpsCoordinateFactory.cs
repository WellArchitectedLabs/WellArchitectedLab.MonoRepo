using MasterData.Client.Dtos.Responses.City.GetAll;
using MasterData.Domain.AggregateModel.Cities.ValueObjects;

namespace MasterData.Api.Factories;

/// <summary>
/// Factory for <see cref="GpsCoordinatesDto"/>
/// </summary>
public static class GpsCoordinatesDtoFactory
{
    /// <summary>
    /// Creates a dto object from a domain <see cref="GpsCoordinates"/> entity
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public static GpsCoordinatesDto
        CreateFromDomain(GpsCoordinates coordinate) =>
        new GpsCoordinatesDto(coordinate.Latitude, coordinate.Longitude);
}    