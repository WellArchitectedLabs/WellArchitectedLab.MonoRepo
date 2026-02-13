using Microsoft.Extensions.Configuration;
using Npgsql;

namespace WeatherInsights.Collector.Infrastructure.DataAccess.Postgres.Connectors;

/// <summary>
/// A generic db factory for a postgres database
/// </summary>
public interface IPostgresDbConnectionFactory
{
    /// <summary>
    /// Creates a new postgres connection
    /// </summary>
    /// <returns></returns>
    NpgsqlConnection CreateConnection();
}

/// <summary>
/// Our implementation for creating a postgres connection object
/// </summary>
public sealed class PostgresDbConnectionFactory : IPostgresDbConnectionFactory
{
    private readonly string _connectionString;
    
    /// <summary>
    /// Initializes the readonly connection string object
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="InvalidOperationException"></exception>

    public PostgresDbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres")
                            ?? throw new InvalidOperationException("Postgres connection string not found.");
    }
    
    /// <summary>
    /// Creates a new connection
    /// </summary>
    /// <returns></returns>
    public NpgsqlConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}