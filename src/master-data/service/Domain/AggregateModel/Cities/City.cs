using MasterData.Domain.AggregateModel.Cities.ValueObjects;

namespace MasterData.Domain.AggregateModel.Cities;

/// <summary>
/// City aggregate root
/// Cities are a central part of weather forecasting
/// The entity is a representation of a world city that is managed by the platform and for which we: 
/// - Gather historical actuals
/// - Calculate forecasting using the weather insights service
/// </summary>
public class City
{
    #region Properties

    /// <summary>
    /// Auto incremented city id
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// City name
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Associated city coordinates. Important for deterministic coordinates-based forecasting
    /// </summary>
    public required GpsCoordinates Coordinates { get; init; }

    #endregion
}