using System.Text.Json;
using Cocona;
using Seederly.Core;

namespace Seederly.Cli;

public class RequestCommands
{
    private readonly HttpClient _httpClient = new();
    private readonly FakeRequestFactory _fakeRequestFactory = new();
    
    public async Task GET(RequestParams requestParams)
    {
        var requests = GenerateRequests(requestParams);
        foreach (var request in requests)
        {
            var response = await SendRequest(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");
        }
    }
    
    public async Task POST(RequestParams requestParams)
    {
        var requests = GenerateRequests(requestParams);
        foreach (var request in requests)
        {
            request.Method = HttpMethod.Post;
            var response = await SendRequest(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");
        }
    }
    
    public async Task PUT(RequestParams requestParams)
    {
        var requests = GenerateRequests(requestParams);
        foreach (var request in requests)
        {
            request.Method = HttpMethod.Put;
            var response = await SendRequest(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");
        }
    }
    
    public async Task DELETE(RequestParams requestParams)
    {
        var requests = GenerateRequests(requestParams);
        foreach (var request in requests)
        {
            request.Method = HttpMethod.Delete;
            var response = await SendRequest(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");
        }
    }
    
    public async Task PATCH(RequestParams requestParams)
    {
        var requests = GenerateRequests(requestParams);
        foreach (var request in requests)
        {
            request.Method = HttpMethod.Patch;
            var response = await SendRequest(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");
        }
    }
    
    public async Task OPTIONS(RequestParams requestParams)
    {
        var requests = GenerateRequests(requestParams);
        foreach (var request in requests)
        {
            request.Method = HttpMethod.Options;
            var response = await SendRequest(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");
        }
    }
    
    public async Task HEAD(RequestParams requestParams)
    {
        var requests = GenerateRequests(requestParams);
        foreach (var request in requests)
        {
            request.Method = HttpMethod.Head;
            var response = await SendRequest(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");
        }
    }
    
    public async Task TRACE(RequestParams requestParams)
    {
        var requests = GenerateRequests(requestParams);
        foreach (var request in requests)
        {
            request.Method = HttpMethod.Trace;
            var response = await SendRequest(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");
        }
    }
    
    public async Task CONNECT(RequestParams requestParams)
    {
        var requests = GenerateRequests(requestParams);
        foreach (var request in requests)
        {
            request.Method = HttpMethod.Connect;
            var response = await SendRequest(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine($"Content: {response.Content}");
        }
    }
    
    
    private List<ApiRequest> GenerateRequests(RequestParams requestParams)
    {
        var requests = new List<ApiRequest>();
        for (int i = 0; i < requestParams.Count; i++)
        {
            var request = new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = requestParams.Url
            };
            
            if (requestParams.Schema != null)
            {
                var map = JsonSerializer.Deserialize<Dictionary<string, string>>(requestParams.Schema);
                request.Body = _fakeRequestFactory.Generate(map).ToJsonString();
            } 
            else if (requestParams.Body != null)
            {
                request.Body = requestParams.Body;
            }
            
            requests.Add(request);
        }

        return requests;
    }
    private async Task<ApiResponse> SendRequest(ApiRequest request)
    {
        var httpRequestMessage = new HttpRequestMessage(request.Method, request.Url);

        if (request.Body != null)
        {
            httpRequestMessage.Content = new StringContent(request.Body);
            if (request.ContentType != null)
            {
                httpRequestMessage.Content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(request.ContentType);
            }
        }

        foreach (var header in request.Headers)
        {
            httpRequestMessage.Headers.Add(header.Key, header.Value);
        }

        var response = await _httpClient.SendAsync(httpRequestMessage);
        return new ApiResponse
        {
            StatusCode = response.StatusCode,
            Content = await response.Content.ReadAsStringAsync()
        };
    }

}

public class RequestParams : ICommandParameterSet
{
    [Argument(Description = "Specifies the URL to send the request to.")]
    public string Url { get; set; }

    [Option('b', Description = "Specifies the body of the request.")]
    [HasDefaultValue]
    public string? Body { get; set; } = null;

    [Option('s', Description = "Specifies the schema to generate fake data with.")]
    [HasDefaultValue]
    public string? Schema { get; set; } = null;

    [Option('c',
        Description =
            "Specifies how many requests will be sent. If using schemas, also how many different schemas will be generated.")]
    [HasDefaultValue]
    public int Count { get; set; } = 1;
}
