using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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

    public bool HasContent => SelectedNode != null && SelectedNode.IsLeaf;

    public WorkspaceViewModel()
    {
        AvailableDataTypes = new(_fakeRequestFactory.Generators.Select(x => x.Key).OrderBy(x => x));
        var requests = new List<ApiRequest>()
        {
            new()
            {
                Name = "Get users",
                Url = "https://httpbin.org/get",
                Method = HttpMethod.Get,
                Headers = { { "Authentication", "Bearer token" } },
                Body = null
            },
            new()
            {
                Name = "Create user",
                Url = "https://httpbin.org/post",
                Method = HttpMethod.Post,
                Headers = { { "Authorization", "Bearer token" } },
                Body = "{\"name\":\"John Doe\",\"email\":\"john@example.com\"}"
            },
            new()
            {
                Name = "Update user",
                Url = "https://httpbin.org/put",
                Method = HttpMethod.Put,
                Headers = { { "Authorization", "Bearer token" } },
                Body = "{\"id\":123,\"email\":\"newemail@example.com\"}"
            },
            new()
            {
                Name = "Delete user",
                Url = "https://httpbin.org/delete",
                Method = HttpMethod.Delete,
                Headers = { { "Authorization", "Bearer token" } },
                Body = null
            }
        };


        Nodes = new ObservableCollection<Node<ApiEndpointModel>>(requests.Select(r => new Node<ApiEndpointModel>(r.Name,
            ApiEndpointModel.FromApiRequest(r))));

        SelectedNode = Nodes.FirstOrDefault();
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
}