using System.Net;

namespace Seederly.Core;

/// <summary>
/// Represents an API response.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Gets or sets the HTTP status code of the response.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }
    
    /// <summary>
    /// Gets or sets the content of the response.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the headers of the response.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();
    
    /// <summary>
    /// Whether or not the response was successful (2xx status code).
    /// </summary>
    public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode < 300;
}