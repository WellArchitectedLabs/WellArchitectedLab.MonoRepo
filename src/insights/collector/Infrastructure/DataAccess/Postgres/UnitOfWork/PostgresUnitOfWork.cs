using Npgsql;
using WeatherInsights.Collector.Domain.Ports.UnitOfWork;
using WeatherInsights.Collector.Infrastructure.DataAccess.Postgres.Connectors;

namespace WeatherInsights.Collector.Infrastructure.DataAccess.Postgres.UnitOfWork;

/// <summary>
/// We need to apply the unit of work pattern to our postgres persistence code
/// </summary>
/// <param name="connectionFactory"><see cref="IPostgresDbConnectionFactory"/></param>
public sealed class PostgresUnitOfWork(IPostgresDbConnectionFactory connectionFactory) : IUnitOfWork
{
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    public NpgsqlConnection Connection => _connection ?? throw new InvalidOperationException("Unit of work not started.");
    public NpgsqlTransaction Transaction => _transaction ?? throw new InvalidOperationException("Unit of work not started.");

    public async Task BeginAsync(CancellationToken cancellationToken)
    {
        _connection =  connectionFactory.CreateConnection();
        await _connection.OpenAsync(cancellationToken);
        _transaction = await _connection.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken) =>
        await Transaction.CommitAsync(cancellationToken);

    public async Task RollbackAsync(CancellationToken cancellationToken) =>
        await Transaction.RollbackAsync(cancellationToken);

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null) await _transaction.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
    }
}