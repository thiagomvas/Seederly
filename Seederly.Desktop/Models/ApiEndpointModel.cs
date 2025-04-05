using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core;
using Seederly.Desktop.ViewModels;

namespace Seederly.Desktop.Models;

public partial class ApiEndpointModel : ViewModelBase
{
    
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private string _method = "GET";
    
    [ObservableProperty]
    private string _url = string.Empty;
    
    [ObservableProperty]
    private string? _body = string.Empty;

    public ObservableCollection<HeaderEntry> Headers { get; set; } = new();
    
    [ObservableProperty]
    private string? _contentType = "application/json";
    
    [ObservableProperty]
    private ApiResponseModel? _lastResponse = new();
    
    public static ApiEndpointModel FromApiRequest(ApiRequest request)
    {
        return new ApiEndpointModel
        {
            Name = request.Name,
            Method = request.Method.ToString(),
            Url = request.Url,
            Body = request.Body,
            Headers = new(request.Headers.Select(kvp => new HeaderEntry(kvp.Key, kvp.Value))),
            ContentType = request.ContentType
        };
    }
    
    public ApiRequest ToApiRequest()
    {
        var request = new ApiRequest
        {
            Name = Name,
            Method = HttpMethod.Parse(Method),
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