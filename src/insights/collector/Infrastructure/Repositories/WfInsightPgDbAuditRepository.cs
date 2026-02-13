using Dapper;
using WeatherInsights.Collector.Domain.AggregateModel.Audit;
using WeatherInsights.Collector.Domain.Ports.Database.Repositories.Interfaces;
using WeatherInsights.Collector.Infrastructure.UnitOfWork;

namespace WeatherInsights.Collector.Infrastructure.Repositories;

public sealed class WfInsightPgDbAuditRepository(PostgresUnitOfWork unitOfWork)
    : IWfInsightAuditRepository
{
    public async Task Save(IEnumerable<WfInsightAudit> wfInsights, CancellationToken cancellationToken)
    {
        var audits = wfInsights as WfInsightAudit[] ?? wfInsights.ToArray();
        if (audits.Length == 0)
            return;

        var requestTimes = audits.Select(a => DateTime.SpecifyKind(a.RequestTimeUtc, DateTimeKind.Utc)).ToArray();
        var responseTimes = audits.Select(a => DateTime.SpecifyKind(a.ResponseTimeUtc, DateTimeKind.Utc)).ToArray();
        var inputs = audits.Select(a => a.WeatherEngineInput).ToArray();
        var outputs = audits.Select(a => a.WeatherEngineOutput).ToArray();
        var insightIds = audits.Select(a => a.WeatherInsightId).ToArray();

        const string sql = """
            INSERT INTO public.wf_insight_audits
                (request_time_utc, response_time_utc, weather_engine_input, weather_engine_output, weather_insight_id)
            SELECT
                t.request_time_utc,
                t.response_time_utc,
                t.weather_engine_input,
                t.weather_engine_output,
                t.weather_insight_id
            FROM UNNEST(
                @RequestTimes,
                @ResponseTimes,
                @Inputs,
                @Outputs,
                @InsightIds
            ) AS t(
                request_time_utc,
                response_time_utc,
                weather_engine_input,
                weather_engine_output,
                weather_insight_id
            );
            """;

        await unitOfWork.Connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    RequestTimes = requestTimes,
                    ResponseTimes = responseTimes,
                    Inputs = inputs,
                    Outputs = outputs,
                    InsightIds = insightIds
                },
                transaction: unitOfWork.Transaction,
                cancellationToken: cancellationToken));
    }
}