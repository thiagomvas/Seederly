using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Seederly.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase currentPage;
    
    public WorkspaceViewModel WorkspaceViewModel ;

    public MainWindowViewModel()
    {
        WorkspaceViewModel = new();
        NavigateToWorkspace();
    }

    [RelayCommand]
    private void NavigateToWorkspace()
    {
        WorkspaceViewModel = new();
        CurrentPage = WorkspaceViewModel;
    }

    [RelayCommand]
    private void NavigateToSettings() => CurrentPage = new SettingsViewModel();
}