
using Cocona;
using Seederly.Cli;
using Seederly.Core;
using Seederly.Core.Automation;

var endpoint = new ApiEndpoint()
{
    Name = "Test",
    Request = new()
    {
        Url = "https://httpbin.org/post",
        Method = HttpMethod.Post,
        Body = @"{
  ""data"": {
    ""token"": ""abc123""
  }
}
",
    }
};

var endpoint2 = new ApiEndpoint()
{
    Name = "Test2",
    Request = new()
    {
        Url = "https://httpbin.org/post",
        Method = HttpMethod.Post,
        Body = @"{
  ""data"": {
    ""token"": ""SECRET TOKEN""
  }
}
",
    }
};

var workflow = new Workflow()
{
    Name = "Test",
    Description = "Test workflow",
    Steps = new List<WorkflowStep>()
    {
        new()
        {
            Name = "Step 1",
            Description = "First step",
            EndpointName = endpoint.Name,
            Extract = new List<VariableExtractionRule>()
            {
                new()
                {
                    JsonPath = "json.data.token",
                    Source = ExtractionVariableTarget.Response,
                    VariableName = "foo"
                }
            },
        },
        new()
        {
            Name = "Step 2",
            Description = "Second step",
            EndpointName = endpoint2.Name,
            Inject = new List<VariableInjectionRule>()
            {
                new()
                {
                    Key = "foo",
                    Target = InjectionVariableTarget.Body,
                    Path = "data.token"
                }
            },
        }
    }
};

var workspace = new Workspace("")
{
    Name = "Test",
    Endpoints = new List<ApiEndpoint>()
    {
        endpoint,
        endpoint2
    },
};

var variableContext = new VariableContext();
var apiRequestExecutor = new ApiRequestExecutor(new HttpClient());

var workflowExecutor = new WorkflowExecutor(apiRequestExecutor, variableContext, workspace);

var workflowResult = await workflowExecutor.ExecuteAsync(workflow);
Console.WriteLine($"Workflow '{workflow.Name}' executed with {workflowResult.Steps.Count} steps.");
foreach (var step in workflowResult.Steps)
{
    Console.WriteLine($"Step '{step.Name}' executed with status: {step.Status}");
    Console.WriteLine($"Response: {step.Response?.Content}");
}

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