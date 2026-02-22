using WfExperience.Bff.Domain.AggregateModel.City;
using WfExperience.Bff.Domain.AggregateModel.Forecast;

namespace WfExperience.Bff.Domain.Ports.Adapters;

/// <summary>
/// Connects to external source for getting master data
/// </summary>
public interface IMasterDataAdapter
{
    /// <summary>
    /// Returns the full cities list of the master data repository
    /// </summary>
    /// <param name="cancellationToken">propagates task cancellations.</param>
    /// <returns></returns>
    Task<List<City>> GetAllManagedCities(CancellationToken cancellationToken);
    
    /// <summary>
    /// Returns the stored weather actuals (real historical data)
    /// For the given time range
    /// </summary>
    /// <param name="cityId">City id</param>
    /// <param name="from">return actuals which timestamp is superior to this date</param>
    /// <param name="to">return actuals which timestamp is inferior to this date</param>
    /// <param name="cancellationToken">used for cancellation propagation</param>
    /// <returns></returns>
    Task<List<WeatherForecast>> GetActuals(
        int cityId, DateTime from, DateTime to, CancellationToken cancellationToken);
}