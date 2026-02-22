using WfExperience.Bff.Application.Services.Interfaces;
using WfExperience.Bff.Domain.AggregateModel.City;
using WfExperience.Bff.Domain.Ports.Adapters;

namespace WfExperience.Bff.Application.Services;

/// <summary>
/// Implementation of <see cref="ICityService"/>
/// </summary>
/// <param name="masterDataAdapter">connects to external source to manage remote cities data</param>
public class CityService(IMasterDataAdapter masterDataAdapter) : ICityService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<City>> GetAll(CancellationToken cancellationToken)
        => await masterDataAdapter.GetAllManagedCities(cancellationToken);
}