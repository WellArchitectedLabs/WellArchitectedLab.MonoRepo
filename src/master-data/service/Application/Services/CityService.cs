using MasterData.Application.Services.Interfaces;
using MasterData.Domain.AggregateModel.Cities;
using MasterData.Domain.Ports;

namespace MasterData.Application.Services;

/// <summary>
/// Applicative service for managing cities
/// </summary>
/// <param name="repository">dependency injected db adapter for connecting to cities database</param>
public class CityService(ICityRepository repository) : ICityService
{
    public Task<IReadOnlyCollection<City>> GetAll(CancellationToken cancellationToken) 
            => repository.GetAll(cancellationToken);
}