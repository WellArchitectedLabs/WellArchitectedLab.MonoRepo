using Dapper;
using WeatherInsights.Collector.Domain.AggregateModel.Insight;
using WeatherInsights.Collector.Domain.Ports.Database.Repositories.Interfaces;
using WeatherInsights.Collector.Infrastructure.UnitOfWork;

namespace WeatherInsights.Collector.Infrastructure.Repositories;

/// <summary>
/// Postgres implementation of <see cref="WfInsight"/> entity data access
/// </summary>
public sealed class WfInsightPgDbRepository(PostgresUnitOfWork unitOfWork) : IWfInsightRepository
{
    /// <inheritdoc/>
    public async Task<ILookup<int, int>> Save(IEnumerable<WfInsight> wfInsights, CancellationToken cancellationToken)
    {
        var insights = wfInsights as WfInsight[] ?? wfInsights.ToArray();
        if (insights.Length == 0)
            return Enumerable.Empty<(int CityId, int Id)>().ToLookup(x => x.CityId, x => x.Id);

        var timestamps = insights.Select(i => DateTime.SpecifyKind(i.TimestampUtc, DateTimeKind.Utc)).ToArray();
        var temperatures = insights.Select(i => i.Temperature).ToArray();
        var windSpeeds = insights.Select(i => i.WindSpeed).ToArray();
        var precipitations = insights.Select(i => i.Precipitation).ToArray();
        var cityIds = insights.Select(i => i.CityId).ToArray();

        const string sql = """
            INSERT INTO public.wf_insights
                (timestamp_utc, temperature, wind_speed, precipitation, city_id)
            SELECT
                t.timestamp_utc,
                t.temperature,
                t.wind_speed,
                t.precipitation,
                t.city_id
            FROM UNNEST(
                @Timestamps,
                @Temperatures,
                @WindSpeeds,
                @Precipitations,
                @CityIds
            ) AS t(
                timestamp_utc,
                temperature,
                wind_speed,
                precipitation,
                city_id
            )
            RETURNING id, city_id;
            """;

        var returned = await unitOfWork.Connection.QueryAsync<(int Id, int CityId)>(
            new CommandDefinition(
                sql,
                new
                {
                    Timestamps = timestamps,
                    Temperatures = temperatures,
                    WindSpeeds = windSpeeds,
                    Precipitations = precipitations,
                    CityIds = cityIds
                },
                transaction: unitOfWork.Transaction,
                cancellationToken: cancellationToken));

        return returned.ToLookup(r => r.CityId, r => r.Id);
    }

    /// <inheritdoc/>
    public async Task<List<WfInsight>> GetInsights(
        int cityId,
        DateTime fromDateTime,
        DateTime toDateTime,
        CancellationToken cancellationToken)
    {
        var fromUtc = DateTime.SpecifyKind(fromDateTime, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(toDateTime, DateTimeKind.Utc);

        const string sql = """
            SELECT
                id              AS Id,
                timestamp_utc   AS TimestampUtc,
                temperature     AS Temperature,
                wind_speed      AS WindSpeed,
                precipitation   AS Precipitation,
                city_id         AS CityId
            FROM public.wf_insights
            WHERE city_id = @CityId
              AND timestamp_utc >= @FromUtc
              AND timestamp_utc <= @ToUtc
            ORDER BY timestamp_utc;
            """;

        var records = await unitOfWork.Connection.QueryAsync<WfInsight>(
            new CommandDefinition(
                sql,
                new
                {
                    CityId = cityId,
                    FromUtc = fromUtc,
                    ToUtc = toUtc
                },
                transaction: unitOfWork.Transaction,
                cancellationToken: cancellationToken));

        return records.AsList();
    }
}