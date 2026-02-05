using MasterData.Domain.AggregateModel.Cities;

namespace MasterData.Domain.Ports;

/// <summary>
/// Interface for all master data access layer
/// Encapsulates database access logic
/// </summary>
public interface ICityRepository
{
    /// <summary>
    /// Connects to data store
    /// Returns all stored database cities
    /// </summary>
    /// <returns></returns>
    public Task<IReadOnlyCollection<City>> GetAll(CancellationToken cancellationToken);
}