using System.Reflection;
using Refit;

namespace MasterData.Client.Formatters;

/// <summary>
/// This formatter is used by Refit for DateOnly type compatibility
/// </summary>
public class DateOnlyUrlParameterFormatter : DefaultUrlParameterFormatter
{
    public override string? Format(object? value, ICustomAttributeProvider attributeProvider, Type type)
    {
        if (value is DateOnly date)
            return date.ToString("yyyy-MM-dd");

        return base.Format(value, attributeProvider, type);
    }
}