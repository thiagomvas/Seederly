using System.Text.Json;
using System.Text.Json.Nodes;
using Seederly.Core;

var mappings = new Dictionary<string, string>
{
    { "Date", "date.past"},
    { "Info", "lorem.sentence"},
    { "User.FirstName", "name.firstName" },
    { "User.LastName", "name.lastName" },
    { "User.Email", "internet.email" },
    { "User.Age", "random.number" },
    { "Adresses[2]", "{\"Street\": \"address.streetAddress\", \"City\": \"address.city\", \"State\": \"address.state\", \"ZipCode\": \"address.zipCode\", \"Country\": \"address.country\"}" },
};

var generator = new FakeRequestFactory();
JsonObject fakeBody = generator.Generate(mappings);

Console.WriteLine(fakeBody.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));