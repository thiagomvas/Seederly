using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Seederly.Core.Configuration;

namespace Seederly.Core;

/// <summary>
/// Client responsible for executing API requests.
/// </summary>
public class ApiRequestExecutor
{
    private readonly HttpClient _httpClient;
    private readonly ILogger? _logger;
    private readonly FakeRequestFactory _fakeRequestFactory;

    public ApiRequestExecutor(HttpClient httpClient, ILogger? logger = null, FakeRequestFactory? fakeRequestFactory = null)
    {
        _fakeRequestFactory = fakeRequestFactory ?? FakeRequestFactory.Instance;
        _logger = logger;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Executes the given API request and returns the response.
    /// </summary>
    /// <param name="request">The API request to execute.</param>
    /// <returns>A <see cref="ApiResponse"/> containing the returned data.</returns>
    public async Task<ApiResponse?> ExecuteAsync(ApiRequest request, StagingEnvironment? environment = null)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            _logger?.LogError("Request URL is null or empty.");
            return null;
        }

        var url = environment?.BuildRoute(request) ?? request.BuildRoute();
            
        var httpRequestMessage = new HttpRequestMessage(request.Method, url);
        
        _fakeRequestFactory.GenerateBody(request);

        if (request.Body != null)
        {
            httpRequestMessage.Content = new StringContent(request.Body, Encoding.UTF8, request.ContentType);
        }

        foreach (var header in request.Headers.Where(h => !string.IsNullOrWhiteSpace(h.Value) && !string.IsNullOrWhiteSpace(h.Key)))
        {
            if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
            {
                if (httpRequestMessage.Content != null)
                {
                    httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(header.Value);
                }
            }
            else
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        //


        var response = await _httpClient.SendAsync(httpRequestMessage);
        var content = await response.Content.ReadAsStringAsync();
        
        _logger?.LogInformation($"[Response] {response.StatusCode} {request.Method} {request.Url}");

        return new ApiResponse
        {
            StatusCode = response.StatusCode,
            Content = content,
            Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value))
        };
    }
    
}