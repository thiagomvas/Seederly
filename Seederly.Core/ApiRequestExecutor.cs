using System.Text;

namespace Seederly.Core;

public class ApiRequestExecutor
{
    private readonly HttpClient _httpClient;

    public ApiRequestExecutor(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse> ExecuteAsync(ApiRequest request)
    {
        var httpRequestMessage = new HttpRequestMessage(request.Method, request.Url);

        if (request.Body != null)
        {
            httpRequestMessage.Content = new StringContent(request.Body, Encoding.UTF8, request.ContentType);
        }

        foreach (var header in request.Headers)
        {
            httpRequestMessage.Headers.Add(header.Key, header.Value);
        }

        var response = await _httpClient.SendAsync(httpRequestMessage);
        var content = await response.Content.ReadAsStringAsync();

        return new ApiResponse
        {
            StatusCode = response.StatusCode,
            Content = content
        };
    }
    
}