namespace MasterData.Client.Dtos.Responses.City.GetAll;

/// <summary>
/// Root dto record for GetAllCities action method
/// </summary>
/// <param name="Id">City Id</param>
/// <param name="Name">City Name</param>
/// <param name="CoordinatesDto">City Coordinates Object</param>
public record CityDto(int Id, string Name, GpsCoordinatesDto CoordinatesDto);

