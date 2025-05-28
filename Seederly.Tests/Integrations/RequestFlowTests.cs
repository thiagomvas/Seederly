using System.Net;
using System.Text.Json.Nodes;
using Seederly.Core;

namespace Seederly.Tests.Integrations;

public class RequestFlowTests
{
    private FakeRequestFactory _fakeRequestFactory;
    private MockHttpClientFactory _mockHttpClientFactory;

    [SetUp]
    public void Setup()
    {
        _fakeRequestFactory = FakeRequestFactory.Instance;
        _mockHttpClientFactory = new MockHttpClientFactory();
    }
    
    [Test]
    public async Task ExecuteRequestFlow_ShouldReturnValidResponse()
    {
        // Arrange
        var request = _fakeRequestFactory.GenerateRequest("https://example.com/api/test", HttpMethod.Get, new() {{"Foo", "name.fullName"}});
        var handler = new MockHttpMessageHandler("{\"message\": \"Success\"}");
        var executor = _mockHttpClientFactory.CreateExecutor(handler);

        // Act
        var response = await executor.ExecuteAsync(request);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.EqualTo("{\"message\": \"Success\"}"));
        });
    }

    [Test]
    public async Task ExecuteRequestFlow_ShouldAutoGenerateFromBody()
    {
        var request = new ApiRequest()
        {
            Url = "https://example.com/api/test",
            Method = HttpMethod.Post,
            Body =
                @"
{
    ""name"": ""{{name.fullName}}""
}
"
        };
        
        var handler = new MockHttpMessageHandler();
        var executor = _mockHttpClientFactory.CreateExecutor(handler);
        
        var response = await executor.ExecuteAsync(request);
        var responseObject = JsonObject.Parse(response.Content);
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(responseObject, Is.Not.Null);
        });
        
        Assert.That(responseObject?["name"]?.GetValue<string>(), Is.Not.Null);
        Assert.That(responseObject?["name"]?.GetValue<string>(), Is.Not.Empty);
    }
}