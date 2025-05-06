using System.Text.Json;

namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents an OpenAPI document, which contains metadata, paths, components, and servers.
/// </summary>
public class OpenApiDocument
{
    /// <summary>
    /// The metadata about the OpenAPI document (e.g., title, version, description).
    /// </summary>
    public OpenApiInfo? Info { get; set; }

    /// <summary>
    /// A dictionary of paths and their corresponding operations.
    /// </summary>
    public Dictionary<string, OpenApiPathItem> Paths { get; set; } = new();

    /// <summary>
    /// The reusable components that are referenced in the OpenAPI document (e.g., schemas, responses).
    /// </summary>
    public OpenApiComponents? Components { get; set; }

    /// <summary>
    /// A list of servers available for the API.
    /// </summary>
    public List<OpenApiServer> Servers { get; set; } = new();

    /// <summary>
    /// Deserializes the provided JSON string into an OpenAPI document.
    /// </summary>
    /// <param name="json">The JSON string representing the OpenAPI document.</param>
    /// <returns>An <see cref="OpenApiDocument"/> object.</returns>
    public static OpenApiDocument FromReferenceJson(string json)
    {
        var document = new OpenApiDocument();
        var jsonObject = JsonSerializer.Deserialize<JsonElement>(json);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (jsonObject.TryGetProperty("info", out var infoElement))
        {
            document.Info = JsonSerializer.Deserialize<OpenApiInfo>(infoElement.GetRawText(), options);
        }

        if (jsonObject.TryGetProperty("paths", out var pathsElement))
        {
            document.Paths = new Dictionary<string, OpenApiPathItem>();

            foreach (var endpoint in pathsElement.EnumerateObject())
            {
                var path = new OpenApiPathItem();
                foreach (var operation in endpoint.Value.EnumerateObject())
                {
                    var operationName = operation.Name.ToLowerInvariant();
                    var operationValue = JsonSerializer.Deserialize<OpenApiOperation>(operation.Value.GetRawText(), options);
                    path.Operations.Add(operationName, operationValue);
                }
                document.Paths.Add(endpoint.Name, path);
            }
            
        }
        if (jsonObject.TryGetProperty("components", out var componentsElement))
        {
            document.Components = JsonSerializer.Deserialize<OpenApiComponents>(componentsElement.GetRawText(), options);
        }

        if (jsonObject.TryGetProperty("servers", out var serversElement))
        {
            document.Servers = JsonSerializer.Deserialize<List<OpenApiServer>>(serversElement.GetRawText(), options);
        }

        return document;
    }
}
