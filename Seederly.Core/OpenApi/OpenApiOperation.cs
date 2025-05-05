namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents an operation in an OpenAPI definition (e.g., GET, POST, PUT).
/// </summary>
public class OpenApiOperation
{
    /// <summary>
    /// A brief summary of the operation.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// A detailed description of the operation.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// A list of parameters for the operation.
    /// </summary>
    public List<OpenApiParameter> Parameters { get; set; } = new();

    /// <summary>
    /// The request body for the operation, if applicable.
    /// </summary>
    public OpenApiRequestBody? RequestBody { get; set; }

    /// <summary>
    /// The responses for the operation, mapped by HTTP status code.
    /// </summary>
    public Dictionary<string, OpenApiResponse> Responses { get; set; } = new();
}
