namespace MasterData.Client.Dtos.Responses.City.GetAll;

/// <summary>
/// Coordinates object, linked to <see cref="GetAllCitiesDto"/>
/// </summary>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
public record GetAllCitiesGpsCoordinatesDto(decimal Latitude, decimal Longitude);