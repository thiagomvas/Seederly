namespace Seederly.Core.Configuration;

public class StagingEnvironment
{
    public string Name { get; set; } = "Staging";
    public string BaseUrl { get; set; } = "https://staging.api.seederly.com";
    public string DocumentationUrl { get; set; } = "https://staging.api.seederly.com";
    
    public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

    public string BuildRoute(ApiRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            throw new ArgumentException("Request URL cannot be null or empty.", nameof(request));

        var url = request.Url.TrimEnd('/');

        url = url.Replace("{{BaseUrl}}", BaseUrl.TrimEnd('/'), StringComparison.OrdinalIgnoreCase);
        
        // Replace {{Variable}} placeholders
        foreach (var variable in Variables)
        {
            url = url.Replace($"{{{{{variable.Key}}}}}", variable.Value, StringComparison.OrdinalIgnoreCase);
        }

        // Add query parameters if any
        if (request.QueryParameters?.Count > 0)
        {
            var queryString = string.Join("&",
                request.QueryParameters.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            url += url.Contains("?") ? "&" : "?";
            url += queryString;
        }

        return new Uri(new Uri(BaseUrl), url).ToString();
    }

}