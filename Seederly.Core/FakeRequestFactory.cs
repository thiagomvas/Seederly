using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Bogus;

namespace Seederly.Core;

/// <summary>
/// A factory class for generating fake API requests.
/// </summary>
public class FakeRequestFactory
{
    private readonly Faker _faker = new();
    public readonly Dictionary<string, Func<object>> Generators;
    
    public static FakeRequestFactory Instance { get; } = new();

    public FakeRequestFactory()
    {
        Generators = new()
        {
            // Name
            ["name.firstName"] = () => _faker.Name.FirstName(),
            ["name.lastName"] = () => _faker.Name.LastName(),
            ["name.fullName"] = () => _faker.Name.FullName(),
            ["name.prefix"] = () => _faker.Name.Prefix(),
            ["name.suffix"] = () => _faker.Name.Suffix(),

            // Internet
            ["internet.email"] = () => _faker.Internet.Email(),
            ["internet.userName"] = () => _faker.Internet.UserName(),
            ["internet.domainName"] = () => _faker.Internet.DomainName(),
            ["internet.url"] = () => _faker.Internet.Url(),
            ["internet.ip"] = () => _faker.Internet.Ip(),
            ["internet.password"] = () => _faker.Internet.PasswordCustom(),

            // Address
            ["address.streetAddress"] = () => _faker.Address.StreetAddress(),
            ["address.city"] = () => _faker.Address.City(),
            ["address.state"] = () => _faker.Address.State(),
            ["address.zipCode"] = () => _faker.Address.ZipCode(),
            ["address.country"] = () => _faker.Address.Country(),
            ["address.latitude"] = () => _faker.Address.Latitude(),
            ["address.longitude"] = () => _faker.Address.Longitude(),

            // Commerce
            ["commerce.productName"] = () => _faker.Commerce.ProductName(),
            ["commerce.price"] = () => _faker.Commerce.Price(),
            ["commerce.department"] = () => _faker.Commerce.Department(),
            ["commerce.ean13"] = () => _faker.Commerce.Ean13(),

            // Random
            ["random.uuid"] = () => Guid.NewGuid().ToString(),
            ["random.number"] = () => _faker.Random.Number(0, 100),
            ["random.word"] = () => _faker.Random.Word(),
            ["random.bool"] = () => _faker.Random.Bool(),
            ["random.alphaNumeric"] = () => _faker.Random.AlphaNumeric(10),
            ["random.hex"] = () => _faker.Random.Hexadecimal(),

            // Date
            ["date.past"] = () => _faker.Date.Past().ToString("o"),
            ["date.future"] = () => _faker.Date.Future().ToString("o"),
            ["date.recent"] = () => _faker.Date.Recent().ToString("o"),
            ["date.soon"] = () => _faker.Date.Soon().ToString("o"),
            ["date.birthday"] = () => _faker.Date.Past(30, DateTime.Now.AddYears(-18)).ToString("o"),

            // Phone
            ["phone.number"] = () => _faker.Phone.PhoneNumber(),

            // Company
            ["company.name"] = () => _faker.Company.CompanyName(),
            ["company.catchPhrase"] = () => _faker.Company.CatchPhrase(),
            ["company.bs"] = () => _faker.Company.Bs(),

            // Finance
            ["finance.account"] = () => _faker.Finance.Account(),
            ["finance.amount"] = () => _faker.Finance.Amount().ToString("F2"),
            ["finance.transactionType"] = () => _faker.Finance.TransactionType(),
            ["finance.iban"] = () => _faker.Finance.Iban(),
            ["finance.bic"] = () => _faker.Finance.Bic(),

            // System
            ["system.fileName"] = () => _faker.System.FileName(),
            ["system.mimeType"] = () => _faker.System.MimeType(),
            ["system.commonFileType"] = () => _faker.System.CommonFileType(),
            ["system.directoryPath"] = () => _faker.System.DirectoryPath(),

            // Lorem
            ["lorem.word"] = () => _faker.Lorem.Word(),
            ["lorem.words"] = () => string.Join(" ", _faker.Lorem.Words(3)),
            ["lorem.sentence"] = () => _faker.Lorem.Sentence(),
            ["lorem.paragraph"] = () => _faker.Lorem.Paragraph(),
        };
    }

    /// <summary>
    /// Generates a fake API request based on the provided URL, HTTP method, and mapping.
    /// </summary>
    /// <param name="url">The request URL</param>
    /// <param name="method">The HTTP Method</param>
    /// <param name="map">The mapping used to generate a body.</param>
    /// <returns>
    /// A <see cref="ApiRequest"/> object containing the generated request.
    /// </returns>
    public ApiRequest GenerateRequest(string url, HttpMethod method,
        Dictionary<string, string> map)
    {
        var request = new ApiRequest
        {
            Url = url,
            Method = method,
            Body = Generate(map).ToJsonString(new JsonSerializerOptions { WriteIndented = true }),
            ContentType = "application/json"
        };

        return request;
    }

    public void GenerateBody(ApiRequest request)
    {
        var regex = new Regex(@"\{\{(.+?)\}\}");
        var matches = regex.Matches(request.Body);
        
        foreach (Match match in matches)
        {
            var key = match.Groups[1].Value;
            if (Generators.TryGetValue(key, out var generator))
            {
                var generatedValue = generator();
                request.Body = request.Body.Replace(match.Value, generatedValue.ToString());
            }
        }
    }
    
    /// <summary>
    /// Generates a JSON object based on the provided mapping.
    /// </summary>
    /// <param name="map">The mapping used to generate the object</param>
    /// <returns>
    /// A <see cref="JsonObject"/> containing the generated JSON object.
    /// </returns>
    public JsonObject Generate(Dictionary<string, string> map)
    {
        var jsonObject = new JsonObject();

        // Separate nested objects into their own mappings
        var nestedObjects = map
            .Where(kvp => kvp.Key.Contains('.'))
            .GroupBy(kvp => kvp.Key.Split('.')[0])
            .ToDictionary(g => g.Key,
                g => g.ToDictionary(kvp => string.Join('.', kvp.Key.Split('.').Skip(1)), kvp => kvp.Value));
        // Add nested objects to the main JSON object
        foreach (var (key, value) in nestedObjects)
        {
            jsonObject[key] = Generate(value);
        }

        var rangeMaps = map
            .Where(kvp => !kvp.Key.Contains('.') && kvp.Key.Contains('['))
            .Select(kvp =>
            {
                var match = Regex.Match(kvp.Key, @"(.+)\[(\d+)\]");
                var key = match.Groups[1].Value;
                var count = int.Parse(match.Groups[2].Value);

                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(kvp.Value);
                    return new Range(key, count, Map: dict);
                }
                catch
                {
                    return new Range(key, count, Generator: kvp.Value);
                }
            })
            .ToList();


        // Add and generate range objects
        foreach (var range in rangeMaps)
        {
            var array = new JsonArray();
            for (var i = 0; i < range.Count; i++)
            {
                if (range.Map is not null)
                {
                    var item = Generate(range.Map);
                    array.Add(item);
                }
                else if (range.Generator is not null)
                {
                    var value = GenerateValue(range.Generator);
                    array.Add(value);
                }
            }

            jsonObject[range.Key] = array;
        }




        var flatMap = map
            .Where(kvp => !kvp.Key.Contains('.') && !kvp.Key.Contains('['))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        foreach (var (key, value) in flatMap)
        {
            jsonObject[key] = JsonSerializer.SerializeToNode(GenerateValue(value));
        }

        return jsonObject;
    }

    private object GenerateValue(string generatorKey)
    {
        if (Generators.TryGetValue(generatorKey, out var generator))
        {
            return generator();
        }

        return string.Empty;
    }

    private record Range(string Key, int Count, Dictionary<string, string>? Map = null, string? Generator = null);

}