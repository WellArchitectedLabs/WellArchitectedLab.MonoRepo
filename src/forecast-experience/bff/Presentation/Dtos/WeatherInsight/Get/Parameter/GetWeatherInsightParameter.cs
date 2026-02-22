namespace WfExperience.Bff.Api.Dtos.WeatherInsight.Get.Parameter;

/// <summary>
/// Input parameter for weather insight controller GET action
/// </summary>
public record GetWeatherInsightParameter
{
    /// <summary>
    /// City to request insights upon
    /// Designated by frontend from the cities list that it would get from the GET cities endpoint.
    /// </summary>
    public int CityId { get; init; }
    
    /// <summary>
    /// Caller (frontend) needs insights starting from this date time (UTC)
    /// From date time = Weather Insight Timestamp (UTC) is superior or equal to this date time
    /// </summary>
    public DateTime FromDate { get; init; }
    
    /// <summary>
    /// Caller (frontend) needs insights ending before this date time (UTC)
    /// To date time = Weather Insight Timestamp (UTC) is inferior or equals to this date time
    /// </summary>
    public DateTime ToDate { get; init; }
}