using System.Diagnostics;
using WeatherInsights.Collector.Domain.AggregateModel.Audit.Enums;

namespace WeatherInsights.Collector.Domain.AggregateModel.Insight.Extensions;

/// <summary>
/// Extensions around enum <see cref="WeatherEngineExecutionStatus"/>
/// </summary>
public static class WeatherEngineExecutionStatusExtensions
{
    public static WeatherEngineExecutionStatus ResolveFromExecutionStatus(bool isExecutionOnError = false, bool isExecutionOnWarning = false)
    {
        if(isExecutionOnError)
            return WeatherEngineExecutionStatus.Ko;
        
        if(isExecutionOnWarning)
            return WeatherEngineExecutionStatus.Degraded;
        
        return WeatherEngineExecutionStatus.Ok;
    }
}