using System.Text.Json;
using System.Text.Json.Serialization;

namespace WeatherInsights.Collector.Infrastructure.HttpClients.JsonConverters;

public sealed class DateTimeKeyDictionaryConverter<TValue>
    : JsonConverter<IDictionary<DateTime, TValue>>
{
    public override IDictionary<DateTime, TValue> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var dict = new Dictionary<DateTime, TValue>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return dict;

            var key = DateTime.Parse(reader.GetString()!, null, System.Globalization.DateTimeStyles.RoundtripKind);

            reader.Read();
            var value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;

            dict.Add(key, value);
        }

        throw new JsonException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        IDictionary<DateTime, TValue> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key.ToString("O"));
            JsonSerializer.Serialize(writer, kvp.Value, options);
        }

        writer.WriteEndObject();
    }
}
