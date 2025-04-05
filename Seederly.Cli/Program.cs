
using Cocona;
using Seederly.Cli;
using Seederly.Core;

var app = CoconaLiteApp.Create();

app.AddCommands<RequestCommands>();
app.AddCommand("schemas", () =>
{
    Console.WriteLine("Available schemas:");
    var fac = new FakeRequestFactory();
    foreach (var generator in fac.Generators)
    {
        Console.WriteLine($"{generator.Key} (E.g.: {generator.Value()})");
    }
});

await app.RunAsync();