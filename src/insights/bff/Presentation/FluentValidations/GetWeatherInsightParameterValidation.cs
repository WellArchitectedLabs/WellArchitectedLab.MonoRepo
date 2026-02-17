using FluentValidation;
using Microsoft.Extensions.Options;
using WeatherInsights.Bff.Api.Dtos.WeatherInsight.Get.Parameter;
using WeatherInsights.Bff.Domain.Ports.Configuration;

namespace WeatherInsights.Bff.Api.FluentValidations;

/// <summary>
/// Fluent validation class for <see cref="GetWeatherInsightParameter"/>
/// </summary>
public class GetWeatherInsightParameterValidation : AbstractValidator<GetWeatherInsightParameter>
{
    /// <summary>
    /// Validation logic for <see cref="GetWeatherInsightParameter"/>
    /// </summary>
    public GetWeatherInsightParameterValidation(IOptionsSnapshot<WeatherInsightsBffEndpointConfig> endpointConfig)
    {
        RuleFor(dto => dto.CityId)
            .NotEqual(0);

        RuleFor(parameter => parameter.ToDate)
            .LessThanOrEqualTo(
                // to date time is inferior to tomorrow's last hour of the day
                new DateTime(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(endpointConfig.Value.GetInsightV1.ToDateMaxDaysFromNow)), 
                        new TimeOnly(23, 59, 59)));

        RuleFor(parameter => parameter.FromDate)
            .LessThanOrEqualTo(parameter => parameter.ToDate);
    }
}