using System.Net;

namespace Seederly.Tests;

public class MockHttpMessageHandler(string responseContent = "", HttpStatusCode statusCode = HttpStatusCode.OK)
    : HttpMessageHandler
{

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string content;
        if (string.IsNullOrWhiteSpace(responseContent))
            content = await request?.Content?.ReadAsStringAsync(cancellationToken) ?? string.Empty;
        else
            content = responseContent;
        
        var response = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content)
        };
        return response;
    }
}