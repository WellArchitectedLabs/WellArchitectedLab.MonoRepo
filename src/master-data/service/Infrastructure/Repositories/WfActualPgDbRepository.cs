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

    public async Task<IReadOnlyCollection<WfActual>> GetByDateTimes(
        IReadOnlyCollection<DateTime> timestampsUtc,
        CancellationToken ct = default)
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
            ORDER BY city_id, timestamp_utc;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var records = await connection.QueryAsync<WfActualRecord>(
            sql,
            new { Timestamps = requested });

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
