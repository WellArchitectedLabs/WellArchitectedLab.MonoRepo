using MasterData.Client.Dtos.Responses.WfActual.Get.History;
using WeatherInsights.Collector.Domain.Extensions;
using WeatherInsights.Collector.Domain.Ports.Config;

namespace WeatherInsights.Collector.Domain.Ports.Clients.Models.Factories;

/// <summary>
/// Factory methods around <see cref="WfEngineInputFactory"/>
/// </summary>
public static class WfEngineInputFactory
{
    /// <summary>
    /// Creates a list of <see cref="WfEngineInput"/> objects from an IEnumerable of <see cref="WfActualDto"/>
    /// </summary>
    /// <param name="wfActuals"></param>
    /// <param name="referenceDate">date subject of calculation</param>
    /// <param name="cityIds">list of city ids, provided as reference data to prediction engine</param>
    /// <param name="masterDataApiParameters">injected from application configs. USed for near / fear years resolution</param>
    /// <returns></returns>
    public static IEnumerable<WfEngineInput> CreateFromWfActuals(
        DateOnly referenceDate,
        IEnumerable<int> cityIds,
        MasterDataApiParameters masterDataApiParameters,
        IEnumerable<WfActualDto> wfActuals)
    {
        var wfActualDtos = wfActuals?.ToList() ?? [];
        var citiesList = cityIds?.ToList() ?? [];
        if (!wfActualDtos.Any())
            throw new ArgumentException("Please provide a non empty wf actuals' list");
        if(masterDataApiParameters == null)
            throw new ArgumentException("Please provide a non null engine config");
        if(!citiesList.Any())
            throw new ArgumentException("Please provide a non empty cities' list");
        var perCityNearActuals = wfActualDtos
            .Where(wfa => wfa.GetDateOnlyTimeStamp() > referenceDate.AddDays(masterDataApiParameters.RollingWindowDays))
            .ToLookup(wfa => wfa.CityId);
        // needed for far actuals calculation
        var nearTimeStamps = perCityNearActuals.SelectMany(kv => 
            kv.Select(wfa => wfa.GetDateOnlyTimeStamp()))
            .ToHashSet();
        var perCityFarActuals = wfActualDtos
            .Where(wfa => nearTimeStamps.Contains(wfa.GetDateOnlyTimeStamp()))
            .ToLookup(wfa => wfa.CityId);
        return citiesList.Select(cityId => new WfEngineInput
        (
            cityId, 
            referenceDate,
            // we'll approximate this to utc now (we accept some nano/milliseconds of delay before the request actually sent via http client)
            RequestTime: DateTime.UtcNow,
            perCityFarActuals[cityId]
                .ToDictionary(wfa => wfa.TimestampUtc, wfa => new WfEngineInputActualItem(wfa)), 
            perCityNearActuals[cityId]
                .ToDictionary(wfa => wfa.TimestampUtc, wfa => new WfEngineInputActualItem(wfa))
        ));
    }
    
}