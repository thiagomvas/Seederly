namespace Seederly.Core.Configuration;

public class StagingEnvironment
{
    public string Name { get; set; } = "Staging";
    public string BaseUrl { get; set; } = "https://staging.api.seederly.com";
    public string DocumentationUrl { get; set; } = "https://staging.api.seederly.com";

    public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

    public string BuildRoute(ApiRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Url))
            throw new ArgumentException("Request URL cannot be null or empty.", nameof(request.Url));
        if (string.IsNullOrWhiteSpace(BaseUrl))
            throw new InvalidOperationException("BaseUrl cannot be null or empty.");

        var trimmedUrl = request.Url.Trim();
        var baseUrlTrimmed = BaseUrl.Trim().TrimEnd('/');

        trimmedUrl = trimmedUrl.Replace("{{BaseUrl}}", baseUrlTrimmed, StringComparison.OrdinalIgnoreCase);

        if (Variables != null)
        {
            foreach (var variable in Variables)
            {
                var placeholder = $"{{{{{variable.Key}}}}}";
                trimmedUrl = trimmedUrl.Replace(placeholder, variable.Value ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        var combinedBase = new Uri(baseUrlTrimmed.EndsWith("/") ? baseUrlTrimmed : baseUrlTrimmed + "/");
        var relativeUri = trimmedUrl.StartsWith("/") ? trimmedUrl.Substring(1) : trimmedUrl;
        var fullUri = new Uri(combinedBase, relativeUri);

        var builder = new UriBuilder(fullUri);

        if (request.QueryParameters != null && request.QueryParameters.Count > 0)
        {
            var query = System.Web.HttpUtility.ParseQueryString(builder.Query ?? "");
            foreach (var kvp in request.QueryParameters)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                    query[kvp.Key] = kvp.Value;
            }

            builder.Query = query.ToString() ?? string.Empty;
        }

        // Remove default ports if any
        if ((builder.Scheme == "https" && builder.Port == 443) ||
            (builder.Scheme == "http" && builder.Port == 80))
        {
            builder.Port = -1;
        }

        return builder.Uri.ToString();
    }
}