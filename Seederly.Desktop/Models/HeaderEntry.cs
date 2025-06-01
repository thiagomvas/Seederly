using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Desktop.ViewModels;

namespace Seederly.Desktop.Models;

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
