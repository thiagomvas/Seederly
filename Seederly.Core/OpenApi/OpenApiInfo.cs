using System.Text.Json.Serialization;

namespace Seederly.Core.OpenApi;

/// <summary>
/// Contains metadata about the OpenAPI document, such as the title and version.
/// </summary>
public class OpenApiInfo
{
    /// <summary>
    /// The title of the OpenAPI document.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The version of the OpenAPI document.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// A description of the OpenAPI document.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
