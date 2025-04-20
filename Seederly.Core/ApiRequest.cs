namespace Seederly.Core;

/// <summary>
/// Represents an API request.
/// </summary>
public class ApiRequest
{
    /// <summary>
    /// Gets or sets the HTTP method for the request.
    /// </summary>
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    
    /// <summary>
    /// Gets or sets the URL for the request.
    /// </summary>
    public string Url { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the body of the request.
    /// </summary>
    public string? Body { get; set; }
    
    /// <summary>
    /// Gets or sets the headers for the request.
    /// </summary>
    /// <remarks>
    /// For Content-Type, use <see cref="ContentType"/> property.
    /// </remarks>
    public Dictionary<string, string> Headers { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the content type for the request.
    /// </summary>
    public string? ContentType { get; set; } = "application/json";
    
    /// <summary>
    /// Gets or sets the last response received from the API.
    /// </summary>
    public ApiResponse? LastResponse { get; set; } = null;
    
    /// <summary>
    /// Adds a JWT Bearer token to the Authorization header of the request.
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    public ApiRequest WithJwt(string jwt)
    {
        Headers["Authorization"] = $"Bearer {jwt}";
        return this;
    }
    public ApiRequest Clone()
    {
        return new ApiRequest
        {
            Method = Method,
            Url = Url,
            Body = Body,
            Headers = new Dictionary<string, string>(Headers, StringComparer.OrdinalIgnoreCase),
            ContentType = ContentType,
            LastResponse = LastResponse
        };
    }
}