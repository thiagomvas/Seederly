namespace Seederly.Tests;

public class MockHttpClientFactory
{
    public HttpClient Create(HttpMessageHandler handler) => new HttpClient(handler);

}
