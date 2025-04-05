using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public ObservableCollection<Node<ApiEndpointModel>> Nodes { get; } = new();
    
    [ObservableProperty]
    private Node<ApiEndpointModel>? _selectedNode;

    public bool HasContent => SelectedNode != null && SelectedNode.IsLeaf;

    public WorkspaceViewModel()
    {
        Nodes.Add(new Node<ApiEndpointModel>("Users", new ObservableCollection<Node<ApiEndpointModel>>
        {
            new("GET - Get Users",
                new ApiEndpointModel()
                {
                    Url = "https://httpbin.org/get",
                    Name = "Get users",
                    Method = "GET",
                    Headers = new([new("a", "b")])
                }),
            new("POST - Create User",
                new ApiEndpointModel()
                {
                    Url = "https://httpbin.org/post", Name = "Create user", Body = @"{ ""name"": ""John Doe"" }",
                    Method = "POST", Headers = new([new("a", "b")])
                }),
            new("PUT - Update User",
                new ApiEndpointModel()
                {
                    Url = "https://httpbin.org/put",
                    Name = "Update user",
                    Body = @"{ ""name"": ""John Doe"" }",
                    Method = "PUT",
                    Headers = new([new("a", "b")])
                }),
            new("DELETE - Delete User",
                new ApiEndpointModel()
                {
                    Url = "https://httpbin.org/delete",
                    Name = "Delete user",
                    Method = "DELETE",
                    Headers = new([new("a", "b")])
                }),
        }));


    }

}