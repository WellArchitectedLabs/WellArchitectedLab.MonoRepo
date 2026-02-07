namespace MasterData.Domain.AggregateModel.Cities.ValueObjects;


/// <summary>
/// A city is positioned by a GPS coordinate
/// Weather forecasting is calculated based on the cities' exact coordinates 
/// Rather than non-deterministic data like city name
/// </summary>
public record struct GpsCoordinates(decimal Longitude, decimal Latitude);