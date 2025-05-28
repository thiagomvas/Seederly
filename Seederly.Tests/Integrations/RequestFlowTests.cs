using System.Net;
using Seederly.Core;

namespace Seederly.Tests.Integrations;

public class RequestFlowTests
{
    [Test]
    public async Task ExecuteRequestFlow_ShouldReturnValidResponse()
    {
        // Arrange
        var request = FakeRequestFactory.Instance.GenerateRequest("https://example.com/api/test", HttpMethod.Get, new() {{"Foo", "name.fullName"}});
        var handler = new MockHttpMessageHandler("{\"message\": \"Success\"}");
        var clientFactory = new MockHttpClientFactory();
        var client = clientFactory.Create(handler);
        var executor = new ApiRequestExecutor(client);

        // Act
        var response = await executor.ExecuteAsync(request);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content, Is.EqualTo("{\"message\": \"Success\"}"));
    }
}