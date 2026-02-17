namespace MasterData.Client.Dtos.Responses.WfActual.Get.History;

public record WfActualDto
{
    /// <summary>
    /// Timestamp associated to the actual value
    /// Actuals are calculated on hourly basis for every day and every city
    /// For uniform calculation, the timestamp is stored in Utc
    /// </summary>
    public required DateTime TimestampUtc { get; init; }
    
    /// <summary>
    /// Celsius based temperature
    /// </summary>
    public required decimal Temperature { get; init; }
    
    /// <summary>
    /// Serves as a parameter serving for the weather forecasting engine
    /// </summary>
    public required decimal WindSpeed { get; init; }
    
    /// <summary>
    /// Serves as a parameter serving for the weather forecasting engine
    /// wiki: https://en.wikipedia.org/wiki/Precipitation
    /// </summary>
    public required decimal Precipitation { get; init; }
    
    /// <summary>
    /// Foreign key to city table
    /// </summary>
    public required int CityId  { get; init; }
}