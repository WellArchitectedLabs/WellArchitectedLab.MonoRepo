namespace WfExperience.Bff.Domain.AggregateModel.City;

/// <summary>
/// A city entity as stored in master data service
/// Adapted to bff frontend exposition needs
/// </summary>
public class City
{
    /// <summary>
    /// Auto incremented city id
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// City name
    /// </summary>
    public required string Name { get; init; }
}