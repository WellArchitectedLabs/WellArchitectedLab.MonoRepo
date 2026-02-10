using WeatherInsights.Collector.Domain.Ports.Clients.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;

namespace WeatherInsights.Collector.Infrastructure.Clients;

public class MockWfEngineClient : IWeatherInsightsEngine
{
    public Task<WfEngineOutput> Call(WfEngineInput wfEngineInput)
    {
        throw new NotImplementedException();
    }
}