using Seederly.Core;

namespace Seederly.Tests;

public class MockHttpClientFactory
{
    public HttpClient Create(HttpMessageHandler handler) => new HttpClient(handler);
    public ApiRequestExecutor CreateExecutor(HttpMessageHandler handler)
    {
        var client = Create(handler);
        return new ApiRequestExecutor(client);
    }
}
