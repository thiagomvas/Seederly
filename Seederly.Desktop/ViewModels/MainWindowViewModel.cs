using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Seederly.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase currentPage;
    
    [ObservableProperty]
    private WorkspaceViewModel _workspaceViewModel ;

    public MainWindowViewModel()
    {
        WorkspaceViewModel = new();
        NavigateToWorkspace();
    }

    [RelayCommand]
    private void SetViewModel(ViewModelBase vm)
    {
        if (vm == null)
            throw new ArgumentNullException(nameof(vm));
        CurrentPage.Dispose();
        
        if(vm is WorkspaceViewModel workspaceViewModel)
            WorkspaceViewModel = workspaceViewModel;

        CurrentPage = vm;
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