using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
    [ObservableProperty] private string _lastResponseStatus = string.Empty;
    [ObservableProperty] private string _lastRequestCalled = string.Empty;

    public bool HasContent => SelectedNode != null && SelectedNode.IsLeaf;

    public WorkspaceViewModel(string workspacePath) : this()
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

        foreach (var node in Nodes)
        {
            node.PropertyChanged += OnAnyPropertyChanged;
            if (node.Value is not null)
                node.Value.PropertyChanged += OnAnyPropertyChanged;
        }

        Nodes.CollectionChanged += OnCollectionChanged;
    }

    [RelayCommand]
    private void AddChildToSelectedNode()
    {
        if (SelectedNode is null)
            return;

        var newNode = new Node<ApiEndpointModel>("New Node", new ApiEndpointModel());

        newNode.PropertyChanged += OnAnyPropertyChanged;
        if (newNode.Value is not null)
            newNode.Value.PropertyChanged += OnAnyPropertyChanged;
        newNode.SubNodes.CollectionChanged += OnCollectionChanged;
        SelectedNode.SubNodes.Add(newNode);
        newNode.Parent = SelectedNode;

        SelectedNode = newNode;
    }


    [RelayCommand(CanExecute = nameof(CanRemoveSelectedNode))]
    private void RemoveSelectedNode()
    {
        if (SelectedNode is null)
            return;

        var parent = SelectedNode.Parent;
        if (parent is not null)
        {
            // Unsubscribe from events
            SelectedNode.PropertyChanged -= OnAnyPropertyChanged;
            if (SelectedNode.Value is not null)
                SelectedNode.Value.PropertyChanged -= OnAnyPropertyChanged;
            SelectedNode.SubNodes.CollectionChanged -= OnCollectionChanged;

            parent.SubNodes.Remove(SelectedNode);
        }
        else
        {
            SelectedNode.PropertyChanged -= OnAnyPropertyChanged;
            if (SelectedNode.Value is not null)
                SelectedNode.Value.PropertyChanged -= OnAnyPropertyChanged;
            SelectedNode.SubNodes.CollectionChanged -= OnCollectionChanged;

            Nodes.Remove(SelectedNode);
        }

        SelectedNode = null;
    }

    private bool CanRemoveSelectedNode() => SelectedNode != null;

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

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < SelectedNode.Amount; i++)
        {
            if (SelectedNode.GenerateEveryRequest)
            {
                Dictionary<string, string> schema = SelectedNode.Value.Schema
                    .Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
                    .ToDictionary(x => x.Key, x => x.Value);
                var body = _fakeRequestFactory.Generate(schema).ToJsonString(new() { WriteIndented = true });
                request.Body = body;
            }
            
            var result = await _apiClient.ExecuteAsync(request);


            SelectedNode.Value.LastResponse = ApiResponseModel.FromApiResponse(result);
            
            LastResponseStatus = $"{(int)result.StatusCode} - {result.StatusCode} ({stopwatch.ElapsedMilliseconds} ms) {(SelectedNode.Amount > 1 ? $"- {i+1}/{SelectedNode.Amount}" : "")}";
            LastRequestCalled = SelectedNode.Name;
        }
        stopwatch.Stop();
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
        var newNode = new Node<ApiEndpointModel>("New Node", new ApiEndpointModel());

        newNode.PropertyChanged += OnAnyPropertyChanged;
        if (newNode.Value is not null)
            newNode.Value.PropertyChanged += OnAnyPropertyChanged;
        newNode.SubNodes.CollectionChanged += OnCollectionChanged;
        newNode.Parent = SelectedNode;

        SelectedNode = newNode;
        Nodes.Add(newNode);
    }

    public void DeserializeWorkspace(string json)
    {
        Node<ApiEndpointModel> ConvertEndpointToNode(ApiEndpoint endpoint)
        {
            var model = ApiEndpointModel.FromApiRequest(endpoint.Request);
            model.Schema = new ObservableCollection<ApiEndpointModel.HeaderEntry>(
                endpoint.Schema.Select(kvp => new ApiEndpointModel.HeaderEntry(kvp.Key, kvp.Value)));
            var node = new Node<ApiEndpointModel>(endpoint.Name, model);
            node.SubNodes.CollectionChanged += OnCollectionChanged;
            node.PropertyChanged += OnAnyPropertyChanged;
            if (model is not null)
                model.PropertyChanged += OnAnyPropertyChanged;
            node.Amount = endpoint.Amount;
            node.GenerateEveryRequest = endpoint.GenerateEveryRequest;

            foreach (var child in endpoint.Children)
            {
                node.SubNodes.Add(ConvertEndpointToNode(child));
                node.SubNodes.Last().Parent = node;
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
                Children = node.SubNodes.Select(ConvertNodeToEndpoint).ToList(),
                Schema = node.Value?.Schema.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, string>(),
                Amount = node.Amount,
                GenerateEveryRequest = node.GenerateEveryRequest
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

    [RelayCommand]
    private async Task ExecuteAllRequests()
    {
        int doneCount = 0;
        var statusCodeCounts = new Dictionary<HttpStatusCode, int>();

        var tasks = Nodes
            .Where(x => x.IsLeaf)
            .Select(node => ExecuteRequest(node.Value!.ToApiRequest()))
            .ToArray();
        int total = tasks.Length;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        LastRequestCalled = "All Requests - Running";
        await foreach (var task in Task.WhenEach(tasks))
        {
            doneCount++;
            var result = await task;

            if (statusCodeCounts.ContainsKey(result.StatusCode))
                statusCodeCounts[result.StatusCode]++;
            else
                statusCodeCounts[result.StatusCode] = 1;

            LastResponseStatus =
                $"{(int)result.StatusCode} - {result.StatusCode} ({doneCount}/{total}) ({stopwatch.ElapsedMilliseconds} ms)";
        }

        stopwatch.Stop();

        string summary = string.Join(", ", statusCodeCounts
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => $"{(int)kvp.Key} {kvp.Key} x{kvp.Value}"));

        LastRequestCalled = "All Requests";
        LastResponseStatus = $"{summary} ({stopwatch.ElapsedMilliseconds} ms)";
    }

    public override void Dispose()
    {
        foreach (var node in Nodes)
        {
            node.PropertyChanged -= OnAnyPropertyChanged;
            if (node.Value is not null)
                node.Value.PropertyChanged -= OnAnyPropertyChanged;
        }

        Nodes.CollectionChanged -= OnCollectionChanged;
    }

    private void OnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (Node<ApiEndpointModel> node in e.NewItems)
            {
                node.PropertyChanged += OnAnyPropertyChanged;
                if (node.Value is not null)
                    node.Value.PropertyChanged += OnAnyPropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (Node<ApiEndpointModel> node in e.OldItems)
            {
                node.PropertyChanged -= OnAnyPropertyChanged;
            }
        }

        SaveWorkspace();
    }

    private void OnAnyPropertyChanged(object? sender, PropertyChangedEventArgs? e)
    {
        SaveWorkspace();
    }

    private void SaveWorkspace()
    {
        if (string.IsNullOrWhiteSpace(WorkspacePath))
            return;

        var json = SerializeWorkspace();
        File.WriteAllText(WorkspacePath, json);
    }

    private async Task<ApiResponse> ExecuteRequest(ApiRequest request)
    {
        var result = await _apiClient.ExecuteAsync(request);

        return result;
    }
}