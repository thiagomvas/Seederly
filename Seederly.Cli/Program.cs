
using Cocona;
using Seederly.Cli;
using Seederly.Core;

var app = CoconaLiteApp.Create();

app.AddCommands<RequestCommands>();
app.AddCommand("schemas", () =>
{
    Console.WriteLine("Available schemas:");
    var fac = new FakeRequestFactory();
    Utils.Write(fac.Generators);
});

await app.RunAsync();