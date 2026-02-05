namespace MasterData.Application.Services.Interfaces;

using MasterData.Domain.AggregateModel.Cities;

/// <summary>
/// Management service related to the <see cref="City"/> entity
/// </summary>
public interface ICityService
{
    /// <summary>
    /// Gets all cities from the data store
    /// Datastore is totally decoupled and injected
    /// Real adapter implementations are located under Infrastructure project  
    /// </summary>
    /// <returns></returns>
    public Task<IReadOnlyCollection<City>> GetAll(CancellationToken cancellationToken);
}