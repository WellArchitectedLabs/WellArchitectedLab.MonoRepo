using Dapper;
using MasterData.Domain.AggregateModel.Cities;
using MasterData.Domain.AggregateModel.Cities.ValueObjects;
using MasterData.Domain.Ports;
using MasterData.Infrastructure.Connectors;
using MasterData.Infrastructure.Repositories.Stubs;

namespace MasterData.Infrastructure.Repositories;

/// <summary>
/// Interface for all master data access layer
/// Encapsulates database access logic
/// </summary>
public sealed class CityPgDbRepository(IPostgresDbConnectionFactory connectionFactory) : ICityRepository
{
    public async Task<IReadOnlyCollection<City>> GetAll(CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, name, latitude, longitude
                           FROM public.cities
                           ORDER BY id;
                           """;

        await using var connection = connectionFactory.CreateConnection();
        var records = await connection.QueryAsync<CityDbStub>(sql, cancellationToken);

        return records.Select(MapToDomain).ToList();
    }

    private static City MapToDomain(CityDbStub record)
        => new()
        {
            Id = record.Id,
            Name = record.Name,
            Coordinates = new GpsCoordinates
            {
                Latitude = record.Latitude,
                Longitude = record.Longitude
            }
        };
}