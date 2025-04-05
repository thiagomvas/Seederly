using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Bogus;

namespace Seederly.Core;

public class FakeRequestFactory
{
    private readonly Faker _faker = new();
    private readonly Dictionary<string, Func<string>> _generators;

    public FakeRequestFactory()
    {
        _generators = new()
        {
            ["name.firstName"] = () => _faker.Name.FirstName(),
            ["name.lastName"] = () => _faker.Name.LastName(),
            ["internet.email"] = () => _faker.Internet.Email(),
            ["commerce.productName"] = () => _faker.Commerce.ProductName(),
            ["commerce.price"] = () => _faker.Commerce.Price(),
            ["random.uuid"] = () => Guid.NewGuid().ToString(),
            ["random.number"] = () => Guid.NewGuid().ToString(),
            ["address.streetAddress"] = () => _faker.Address.StreetAddress(),
            ["address.city"] = () => _faker.Address.City(),
            ["address.state"] = () => _faker.Address.State(),
            ["address.zipCode"] = () => _faker.Address.ZipCode(),
            ["address.country"] = () => _faker.Address.Country(),
            ["date.past"] = () => _faker.Date.Past().ToString("o"),
            ["date.future"] = () => _faker.Date.Future().ToString("o"),
            ["lorem.sentence"] = () => _faker.Lorem.Sentence(),
            ["lorem.paragraph"] = () => _faker.Lorem.Paragraph()
        };
    }

    public JsonObject Generate(Dictionary<string, string> map)
    {
        var jsonObject = new JsonObject();

        foreach (var (key, value) in map)
        {
            jsonObject[key] = GenerateValue(value);
        }
        
        return jsonObject;
    }
    
    private string GenerateValue(string generatorKey)
    {
        if(_generators.TryGetValue(generatorKey, out var generator))
        {
            return generator();
        }

        return string.Empty;
    }


}

