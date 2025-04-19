using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seederly.Core;
using Seederly.Desktop.Models;

namespace Seederly.Desktop.ViewModels;

public partial class WorkspaceViewModel : ViewModelBase
{
    private readonly ApiRequestExecutor _apiClient = new(new HttpClient());
    private readonly FakeRequestFactory _fakeRequestFactory = new();
    public ObservableCollection<Node<ApiEndpointModel>> Nodes { get; } = new();

    public ObservableCollection<string> AvailableDataTypes { get; } = new();

    [ObservableProperty] private Node<ApiEndpointModel>? _selectedNode;
    [ObservableProperty] private string? _workspacePath;
    [ObservableProperty] private string _workspaceName = "New Workspace";

    public bool HasContent => SelectedNode != null && SelectedNode.IsLeaf;

    public WorkspaceViewModel(string workspacePath) : base()
    {
        _workspacePath = workspacePath;
        if (string.IsNullOrWhiteSpace(workspacePath))
            return;
        
        var json = File.ReadAllText(workspacePath);
        DeserializeWorkspace(json);
        SelectedNode = Nodes.FirstOrDefault();
    }

    public WorkspaceViewModel()
    {
        AvailableDataTypes = new(_fakeRequestFactory.Generators.Select(x => x.Key).OrderBy(x => x));
    }

    [RelayCommand]
    private void RemoveSchemaRow(ApiEndpointModel.HeaderEntry schema)
    {
        if (SelectedNode == null || !SelectedNode.IsLeaf)
            return;

        SelectedNode.Value.Schema.Remove(schema);
    }

    [RelayCommand]
    private void AddHeader()
    {
        if (SelectedNode == null || !SelectedNode.IsLeaf)
            return;

        var newHeader = new ApiEndpointModel.HeaderEntry(string.Empty, string.Empty);
        SelectedNode.Value.Headers.Add(newHeader);
    }

    [RelayCommand]
    private void RemoveHeader(ApiEndpointModel.HeaderEntry header)
    {
        if (SelectedNode == null || !SelectedNode.IsLeaf)
            return;

        SelectedNode.Value.Headers.Remove(header);
    }

    [RelayCommand]
    private void AddSchemaColumn()
    {
        if (SelectedNode == null || !SelectedNode.IsLeaf)
            return;

        var newSchema = new ApiEndpointModel.HeaderEntry(string.Empty, string.Empty);
        SelectedNode.Value.Schema.Add(newSchema);
    }

    [RelayCommand]
    private async Task ExecuteRequest()
    {
        if (SelectedNode == null || !SelectedNode.IsLeaf)
            return;

        var request = SelectedNode.Value.ToApiRequest();
        var result = await _apiClient.ExecuteAsync(request);

        SelectedNode.Value.LastResponse = ApiResponseModel.FromApiResponse(result);
    }

    [RelayCommand]
    private void GenerateBody()
    {
        if (SelectedNode == null || !SelectedNode.IsLeaf)
            return;

        var map = SelectedNode.Value.Schema
            .Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

        var body = _fakeRequestFactory.Generate(map).ToJsonString(new() { WriteIndented = true });

        SelectedNode.Value.Body = body;
    }
    
    [RelayCommand]
    private void AddNode()
    {
        if (SelectedNode == null)
            return;

        var newNode = new Node<ApiEndpointModel>("New Endpoint", new ApiEndpointModel());
        SelectedNode.SubNodes.Add(newNode);
        SelectedNode = newNode;
    }
    public void DeserializeWorkspace(string json)
    {
        Node<ApiEndpointModel> ConvertEndpointToNode(ApiEndpoint endpoint)
        {
            var model = ApiEndpointModel.FromApiRequest(endpoint.Request);
            var node = new Node<ApiEndpointModel>(endpoint.Name, model);

            foreach (var child in endpoint.Children)
            {
                node.SubNodes.Add(ConvertEndpointToNode(child));
            }

            return node;
        }

        var workspace = JsonSerializer.Deserialize<Workspace>(json);

        if (workspace is null)
            return;

        WorkspaceName = workspace.Name;
        WorkspacePath = workspace.Path;
        Nodes.Clear();

        foreach (var endpoint in workspace.Endpoints)
        {
            Nodes.Add(ConvertEndpointToNode(endpoint));
        }
    }


    public string SerializeWorkspace()
    {
        ApiEndpoint ConvertNodeToEndpoint(Node<ApiEndpointModel> node)
        {
            var endpoint = new ApiEndpoint
            {
                Name = node.Name,
                Request = node.Value?.ToApiRequest() ?? new ApiRequest(),
                Children = node.SubNodes.Select(ConvertNodeToEndpoint).ToList()
            };

            return endpoint;
        }

        var workspace = new Workspace(WorkspaceName)
        {
            Path = WorkspacePath,
            Endpoints = Nodes.Select(ConvertNodeToEndpoint).ToList()
        };
        

        return JsonSerializer.Serialize(workspace, new JsonSerializerOptions
        {
            WriteIndented = true // Optional, for readability
        });
    }


}