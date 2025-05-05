using Cocona;
using Seederly.Cli;
using Seederly.Core;
using Seederly.Core.Automation;
using Seederly.Core.OpenApi;

var httpClient = new HttpClient();
var json = await httpClient.GetStringAsync(@"https://raw.githubusercontent.com/stoplightio/Public-APIs/refs/heads/master/reference/pdfgenerator/pdfgeneratorapi.json");

var openApiDocument = OpenApiDocument.FromReferenceJson(json);
Console.WriteLine($"Title: {openApiDocument.Info.Title}");

return;
var app = CoconaLiteApp.Create();

app.AddCommands<RequestCommands>();
app.AddCommand("schemas", () =>
{
    Console.WriteLine("Available schemas:");
    var fac = new FakeRequestFactory();
    Utils.Write(fac.Generators);
});

await app.RunAsync();