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
    
    public Dictionary<string, string> QueryParameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    
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
            LastResponse = LastResponse,
            QueryParameters = new Dictionary<string, string>(QueryParameters, StringComparer.OrdinalIgnoreCase)
        };
    }
    
    public Dictionary<string, string> GetHeaders()
    {
        var headers = new Dictionary<string, string>(Headers, StringComparer.OrdinalIgnoreCase);
        
        // Ensure Content-Type is set if not already present
        if (!headers.ContainsKey("Content-Type") && !string.IsNullOrEmpty(ContentType))
        {
            headers["Content-Type"] = ContentType;
        }
        
        return headers;
    }
    
    public string BuildRoute()
    {
        if (string.IsNullOrWhiteSpace(Url))
            throw new InvalidOperationException("Url cannot be null or empty.");

        var uriBuilder = new UriBuilder(Url.Trim());

        if (QueryParameters != null && QueryParameters.Count > 0)
        {
            var existingQuery = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query ?? "");

            foreach (var kvp in QueryParameters)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                    existingQuery[kvp.Key] = kvp.Value ?? string.Empty;
            }

            uriBuilder.Query = existingQuery.ToString() ?? string.Empty;
        }
        else
        {
            uriBuilder.Query = string.Empty;
        }

        // Remove default ports if any
        if ((uriBuilder.Scheme == "https" && uriBuilder.Port == 443) ||
            (uriBuilder.Scheme == "http" && uriBuilder.Port == 80))
        {
            uriBuilder.Port = -1;
        }

        return uriBuilder.Uri.ToString();
    }

}