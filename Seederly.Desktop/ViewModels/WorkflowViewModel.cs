using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core;
using Seederly.Core.Automation;
using Seederly.Desktop.Models;

namespace Seederly.Desktop.ViewModels;

public partial class WorkflowViewModel : ViewModelBase
{

    [ObservableProperty] private WorkflowModel _workflow;

    private readonly WorkflowExecutor _executor;
    private readonly VariableContext _variableContext;
    private readonly ApiRequestExecutor _apiRequestExecutor;
    
    public WorkflowViewModel()
    {
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
                Method = HttpMethod.Put,
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

        Workflow = WorkflowModel.FromWorkflow(workflow);
        _apiRequestExecutor = new ApiRequestExecutor(new HttpClient());
        _variableContext = new VariableContext();
        _executor = new WorkflowExecutor(_apiRequestExecutor, _variableContext, workspace);
    }
    
}