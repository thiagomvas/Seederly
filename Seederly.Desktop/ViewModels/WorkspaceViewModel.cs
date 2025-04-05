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
    public ObservableCollection<Node<ApiEndpointModel>> Nodes { get; } = new();
    
    [ObservableProperty]
    private Node<ApiEndpointModel>? _selectedNode;

    public bool HasContent => SelectedNode != null && SelectedNode.IsLeaf;

    public WorkspaceViewModel()
    {
        var requests = new List<ApiRequest>()
        {
            new() 
            {
                Name = "Get users", 
                Url = "https://httpbin.org/get", 
                Method = HttpMethod.Get, 
                Headers = {{"Authentication", "Bearer token"}}, 
                Body = null
            },
            new() 
            {
                Name = "Create user", 
                Url = "https://httpbin.org/post", 
                Method = HttpMethod.Post, 
                Headers = {{"Authorization", "Bearer token"}}, 
                Body = "{\"name\":\"John Doe\",\"email\":\"john@example.com\"}"
            },
            new() 
            {
                Name = "Update user", 
                Url = "https://httpbin.org/put", 
                Method = HttpMethod.Put, 
                Headers = {{"Authorization", "Bearer token"}}, 
                Body = "{\"id\":123,\"email\":\"newemail@example.com\"}"
            },
            new() 
            {
                Name = "Delete user", 
                Url = "https://httpbin.org/delete", 
                Method = HttpMethod.Delete, 
                Headers = {{"Authorization", "Bearer token"}}, 
                Body = null
            }
        };


        Nodes = new ObservableCollection<Node<ApiEndpointModel>>(requests.Select(r => new Node<ApiEndpointModel>(r.Name,
            ApiEndpointModel.FromApiRequest(r))));
        
        SelectedNode = Nodes.FirstOrDefault();
    }
    
    [RelayCommand]
    private async Task ExecuteRequest()
    {
        if (SelectedNode == null || !SelectedNode.IsLeaf)
            return;
        
        var request = SelectedNode.Value.ToApiRequest();
        var result = await _apiClient.ExecuteAsync(request);
        
        if (result.IsSuccess)
        {
            SelectedNode.Value.LastResponse = ApiResponseModel.FromApiResponse(result);
        }
    }

}