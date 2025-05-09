using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core;
using Seederly.Desktop.ViewModels;

namespace Seederly.Desktop.Models;

public partial class ApiResponseModel : ViewModelBase
{
    [ObservableProperty]
    private string _statusCode = string.Empty;
    [ObservableProperty]
    private string _content = string.Empty;
    
    public ObservableCollection<ApiEndpointModel.HeaderEntry> Headers { get; set; } = new();
    
    public static ApiResponseModel FromApiResponse(ApiResponse response)
    {
        return new ApiResponseModel
        {
            StatusCode = response.StatusCode.ToString(),
            Content = response.Content,
            Headers = new ObservableCollection<ApiEndpointModel.HeaderEntry>(response.Headers.Select(kvp => new ApiEndpointModel.HeaderEntry(kvp.Key, kvp.Value)))
        };
    }
    
    
}