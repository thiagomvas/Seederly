using System.Text.Json;
using System.Text.Json.Nodes;
using Seederly.Core;

var mappings = new Dictionary<string, string>
{
    { "Id", "random.uuid" },
    { "Timestamp", "date.past" },
    { "Info", "lorem.paragraph" },

    { "User.FirstName", "name.firstName" },
    { "User.LastName", "name.lastName" },
    { "User.Email", "internet.email" },
    { "User.Age", "random.number" },
    { "User.Profile.Username", "internet.userName" },
    { "User.Profile.AvatarUrl", "internet.url" },

    { "User.Contact.Phone", "phone.number" },
    { "User.Contact.Address.Street", "address.streetAddress" },
    { "User.Contact.Address.City", "address.city" },
    { "User.Contact.Address.ZipCode", "address.zipCode" },

    // Array of objects
    { "Addresses[3]", @"
        {
            ""Street"": ""address.streetAddress"",
            ""City"": ""address.city"",
            ""State"": ""address.state"",
            ""ZipCode"": ""address.zipCode"",
            ""Country"": ""address.country""
        }"
    },

    // Nested array inside a user
    { "User.PreviousOrders[2]", @"
        {
            ""OrderId"": ""random.uuid"",
            ""Amount"": ""commerce.price"",
            ""Date"": ""date.past"",
            ""Product"": ""commerce.productName""
        }"
    },
};


var generator = new FakeRequestFactory();
JsonObject fakeBody = generator.Generate(mappings);

Console.WriteLine(fakeBody.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));