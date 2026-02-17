using FluentValidation;
using Microsoft.Extensions.Options;
using WeatherInsights.Collector.Client.Dtos.Parameters.Get;
using WeatherInsights.Collector.Domain.Ports.Config;

namespace WfInsights.Collector.Api.FluentValidations;

/// <summary>
/// Fluent validation object for <see cref="GetWeatherInsightParameters"/>
/// Fluent validation docs: https://docs.fluentvalidation.net/en/latest/custom-validators.html
/// <see cref="AbstractValidator{T}"/> Installed from nuget package: FluentValidation
/// </summary>
public class GetWeatherInsightParametersValidator : AbstractValidator<GetWeatherInsightParameters>
{
    public GetWeatherInsightParametersValidator(IOptionsSnapshot<EndpointsConfig> endpointsConfig)
    {
        var maxGetEndpointRange = endpointsConfig.Value.GetInsightsV1.MaxRequestDateRangeInDays;
        RuleFor(dto => dto.FromDateTime).NotNull();
        RuleFor(dto => dto.ToDateTime).NotNull();
        RuleFor(dto => dto.ToDateTime).GreaterThan(dto => dto.FromDateTime);
        RuleFor(x => x)
            .Must(o => HaveValidDateRange(o, 30))
            .WithMessage("The date range must not exceed 30 days.");
    }
    
    /// <summary>
    /// Validates the difference in days between from date time and two date time
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="maxRangeInDays"></param>
    /// <returns></returns>
    private static bool HaveValidDateRange(GetWeatherInsightParameters parameter, int maxRangeInDays)
        => (parameter.ToDateTime - parameter.FromDateTime).TotalDays <= maxRangeInDays;
}