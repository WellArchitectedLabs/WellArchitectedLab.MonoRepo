using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WeatherInsights.Collector.Domain.Ports.Clients.Interfaces;
using WeatherInsights.Collector.Domain.Ports.Clients.Models;
using WeatherInsights.Collector.Infrastructure.Clients.JsonConverters;

namespace WeatherInsights.Collector.Infrastructure.Clients;

/// <summary>
/// Connecting with prediction service, calls it, deserializes the response
/// </summary>
/// <param name="httpClientFactory"></param>
/// <param name="logger"></param>
public class WfEngineClient(
    HttpClient client) : IWeatherInsightsEngine
{

    const string LaunchForecastUri="/forecast";
    
    /// <inheritdoc/>
    public async Task<WfEngineOutput> Call(WfEngineInput wfEngineInput, CancellationToken cancellationToken)
    {
        // Create a memory-efficient stream
        var stream = new MemoryStream();

        // Wrap it with GZip for compression
        using (var gzip = new GZipStream(stream, CompressionLevel.Optimal, leaveOpen: true))
        {
            // Serialize the payload directly to the compressed stream
            await JsonSerializer.SerializeAsync(gzip, wfEngineInput, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            });
        }

        // Reset stream position
        stream.Seek(0, SeekOrigin.Begin);

        // Wrap in StreamContent for HttpClient
        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        content.Headers.ContentEncoding.Add("gzip"); // Important: tell server it's gzipped

        // Send request
        var response = await client.PostAsync(LaunchForecastUri, content, cancellationToken);

        // throws exception is status code is not successful
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync();

        Stream jsonStream = responseStream;

        if (response.Content.Headers.ContentEncoding.Contains("gzip"))
        {
            jsonStream = new GZipStream(responseStream, CompressionMode.Decompress);
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new DateOnlyJsonConverter());
        options.Converters.Add(new DateTimeKeyDictionaryConverter<WfEngineInsightOutput>());

        var result = await JsonSerializer.DeserializeAsync<WfEngineOutput>(jsonStream, options);

        if (result is null)
            throw new InvalidOperationException("Failed to deserialize WfEngineOutput.");

        return result;
    }
}