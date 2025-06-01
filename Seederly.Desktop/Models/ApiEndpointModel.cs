using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core;

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
    public ObservableCollection<HeaderEntry> QueryParams { get; set; } = new();
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
        return method.Method.ToUpperInvariant() switch
        {
            "GET" => 0,
            "POST" => 1,
            "PUT" => 2,
            "DELETE" => 3,
            "PATCH" => 4,
            "HEAD" => 5,
            "OPTIONS" => 6,
            "TRACE" => 7,
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
            4 => HttpMethod.Patch,
            5 => HttpMethod.Head,
            6 => HttpMethod.Options,
            7 => HttpMethod.Trace,
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
        
        foreach (var queryParam in QueryParams)
        {
            request.QueryParameters[queryParam.Key] = queryParam.Value;
        }
        
        return request;
    }
}