using Dapper;
using MasterData.Domain.AggregateModel.Actuals;
using MasterData.Domain.Ports;
using MasterData.Infrastructure.Connectors;
using MasterData.Infrastructure.Repositories.Stubs;
using Microsoft.Extensions.Logging;

namespace MasterData.Infrastructure.Repositories;

/// <summary>
/// Postgres implementation of wf actual data access layer
/// </summary>
public sealed class WfActualPgDbRepository : IWfActualRepository
{
    private readonly IPostgresDbConnectionFactory _connectionFactory;
    private readonly ILogger<WfActualPgDbRepository> _logger;
    
    /// <summary>
    /// Initializes repository dependencies
    /// </summary>
    /// <param name="connectionFactory"></param>
    /// <param name="logger"></param>
    public WfActualPgDbRepository(
        IPostgresDbConnectionFactory connectionFactory,
        ILogger<WfActualPgDbRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public Task<IReadOnlyCollection<WfActual>> GetByDateTimes(
        IReadOnlyCollection<DateTime> timestampsUtc,
        CancellationToken ct = default)
            => GetFromParameters(null, timestampsUtc, ct);
    
    /// <inheritdoc/>
    public Task<IReadOnlyCollection<WfActual>> GetByDateTimes(
        int cityId, 
        IReadOnlyCollection<DateTime> timestampsUtc, 
        CancellationToken ct)
            => GetFromParameters(cityId, timestampsUtc, ct);
    
    /// <summary>
    /// Gets 
    /// </summary>
    /// <param name="cityId">nullable city id. Caller should not provide if we don't want to filter on cities</param>
    /// <param name="timestampsUtc">list of timestamps to get weather actuals</param>
    /// <param name="ct">cancellation token to propagate</param>
    /// <returns></returns>
    private async Task<IReadOnlyCollection<WfActual>> GetFromParameters(
        int? cityId,
        IReadOnlyCollection<DateTime> timestampsUtc, 
        CancellationToken ct)
    {
        if (timestampsUtc.Count == 0)
            return Array.Empty<WfActual>();

        // Normalize and de-duplicate
        var requested = timestampsUtc
            .Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc))
            .Distinct()
            .ToArray();

        const string sql = """
                           SELECT
                               timestamp_utc   AS TimestampUtc,
                               temperature_c   AS Temperature,
                               wind_speed      AS WindSpeed,
                               precipitation   AS Precipitation,
                               city_id         AS CityId
                           FROM public.wf_actuals
                           WHERE timestamp_utc = ANY(@Timestamps)
                           AND (@CityId IS NULL OR city_id = @CityId)
                           ORDER BY city_id, timestamp_utc;
                           """;

        await using var connection = _connectionFactory.CreateConnection();
        
        await connection.ExecuteAsync("SET TIME ZONE 'UTC';");

        var records = await connection.QueryAsync<WfActualRecord>(
            sql,
            new { Timestamps = requested, CityId = cityId });

        var actuals = records
            .Select(MapToDomain)
            .ToArray();

        return actuals;
    }
    
    
    /// <summary>
    /// Maps a db record to a domain object
    /// </summary>
    /// <param name="record">wf actuals db object</param>
    /// <returns></returns>
    private static WfActual MapToDomain(WfActualRecord record)
        => new()
        {
            TimestampUtc = DateTime.SpecifyKind(record.TimestampUtc, DateTimeKind.Utc),
            Temperature = record.Temperature,
            WindSpeed = record.WindSpeed,
            Precipitation = record.Precipitation,
            CityId = record.CityId
        };
}
