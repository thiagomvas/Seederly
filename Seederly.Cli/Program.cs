using Seederly.Core;

var executor = new ApiRequestExecutor(new());

var request = new ApiRequest
{
    Name = "Get Users",
    Method = HttpMethod.Get,
    Url = "https://jsonplaceholder.typicode.com/users",
    Headers =
    {
        { "Accept", "application/json" }
    }
};

var response = await executor.ExecuteAsync(request);
Console.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
Console.WriteLine($"Is Success: {response.IsSuccess}");
Console.WriteLine($"Response Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
Console.WriteLine($"Response Content: {response.Content}");
