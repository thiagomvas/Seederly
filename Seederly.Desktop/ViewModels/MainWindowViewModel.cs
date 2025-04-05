using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Seederly.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase currentPage;

    public MainWindowViewModel()
    {
        CurrentPage = new WorkspaceViewModel();
    }

    [RelayCommand]
    private void NavigateToWorkspace() => CurrentPage = new WorkspaceViewModel();

    [RelayCommand]
    private void NavigateToSettings() => CurrentPage = new SettingsViewModel();
}