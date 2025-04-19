using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core;
using Seederly.Desktop.ViewModels;

namespace Seederly.Desktop.Models;

public partial class ApiEndpointModel : ObservableObject
{
    [ObservableProperty]
    private int _method;
    
    [ObservableProperty]
    private string _url = string.Empty;
    
    [ObservableProperty]
    private string? _body = string.Empty;

    public ObservableCollection<HeaderEntry> Headers { get; set; } = new();
    public ObservableCollection<HeaderEntry> Schema { get; set; } = new();
    
    [ObservableProperty]
    private string? _contentType = "application/json";
    
    [ObservableProperty]
    private ApiResponseModel? _lastResponse = new();
    
    public static ApiEndpointModel FromApiRequest(ApiRequest request)
    {
        return new ApiEndpointModel
        {
            Method = FromMethod(request.Method),
            Url = request.Url,
            Body = request.Body,
            Headers = new(request.Headers.Select(kvp => new HeaderEntry(kvp.Key, kvp.Value))),
            ContentType = request.ContentType
        };
    }
    
    public static int FromMethod(HttpMethod method)
    {
        return method.Method switch
        {
            "GET" => 0,
            "POST" => 1,
            "PUT" => 2,
            "DELETE" => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(method), "Invalid HTTP method")
        };
    }

    public static HttpMethod MapMethod(int method)
    {
        return method switch
        {
            0 => HttpMethod.Get,
            1 => HttpMethod.Post,   
            2 => HttpMethod.Put,
            3 => HttpMethod.Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(method), "Invalid HTTP method")
        };
    }
    
    public ApiRequest ToApiRequest()
    {
        var request = new ApiRequest
        {
            Method = MapMethod(Method),
            Url = Url,
            Body = Body,
            ContentType = ContentType
        };
        
        foreach (var header in Headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }
        
        return request;
    }

    public partial class HeaderEntry : ViewModelBase
    {
        [ObservableProperty]
        private string _key = string.Empty;

        [ObservableProperty]
        private string _value = string.Empty;

        public HeaderEntry(string key, string value)
        {
            _key = key;
            _value = value;
        }
    }

}