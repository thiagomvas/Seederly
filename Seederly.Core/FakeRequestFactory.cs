using System.Text.Json;
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
        
        // Separate nested objects into their own mappings
        var nestedObjects = map
            .Where(kvp => kvp.Key.Contains('.'))
            .GroupBy(kvp => kvp.Key.Split('.')[0])
            .ToDictionary(g => g.Key, g => g.ToDictionary(kvp => kvp.Key.Split('.')[1], kvp => kvp.Value));
        // Add nested objects to the main JSON object
        foreach (var (key, value) in nestedObjects)
        {
            jsonObject[key] = Generate(value);
        }
        
        var rangeMaps = map
            .Where(kvp => kvp.Key.Contains('['))
            .Select(kvp =>
            {
                var match = Regex.Match(kvp.Key, @"(.+)\[(\d+)\]");
                return new Range(match.Groups[1].Value, 
                    int.Parse(match.Groups[2].Value), 
                    JsonSerializer.Deserialize<Dictionary<string, string>>(kvp.Value));
            })
            .ToList();
        
        // Add and generate range objects
        foreach (var range in rangeMaps)
        {
            var array = new JsonArray();
            for (var i = 0; i < range.Count; i++)
            {
                var item = Generate(range.Map);
                array.Add(item);
            }
            jsonObject[range.Key] = array;
        }
        
        
        var flatMap = map
            .Where(kvp => !kvp.Key.Contains('.') && !kvp.Key.Contains('['))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        foreach (var (key, value) in flatMap)
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

    private record Range(string Key, int Count, Dictionary<string, string> Map);
}

