namespace MasterData.Infrastructure.Repositories.Stubs;

/// <summary>
/// Db object for a Wf Actual
/// </summary>
internal sealed class WfActualRecord
{
    public DateTime TimestampUtc { get; init; }
    public decimal Temperature { get; init; }
    public decimal WindSpeed { get; init; }
    public decimal Precipitation { get; init; }
    public int CityId { get; init; }
}
