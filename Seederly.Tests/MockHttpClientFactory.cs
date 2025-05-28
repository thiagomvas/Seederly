using Seederly.Core;

namespace Seederly.Tests;

public class MockHttpClientFactory
{
    public HttpClient Create(HttpMessageHandler handler) => new HttpClient(handler);
    public ApiRequestExecutor CreateExecutor(HttpMessageHandler handler, ILogger? logger = null)
    {
        var client = Create(handler);
        return new ApiRequestExecutor(client, logger, FakeRequestFactory.Instance);
    }
}
