using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seederly.Core;
using Seederly.Core.Automation;

namespace Seederly.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public static MainWindowViewModel? Instance { get; private set; }
    
    [ObservableProperty] private string _workspaceName = "New Workspace";
    [ObservableProperty] private string _lastOperation = "No operations performed yet.";
    [ObservableProperty] private string _status = "Ready";

    [ObservableProperty]
    private ViewModelBase? currentPage;
    
    [ObservableProperty]
    private WorkspaceViewModel _workspaceViewModel ;
    
    [ObservableProperty]
    private WorkflowViewModel _workflowViewModel;

    public Workspace? LoadedWorkspace { get; set; } = new Workspace("New Workspace");

    public MainWindowViewModel()
    {
        WorkspaceViewModel = new();
        NavigateToWorkspace();
        Instance = this;
    }

    [RelayCommand]
    public void SetViewModel(ViewModelBase vm)
    {
        if (vm == null)
            throw new ArgumentNullException(nameof(vm));
        CurrentPage?.Dispose();
        
        if(vm is WorkspaceViewModel workspaceViewModel)
            WorkspaceViewModel = workspaceViewModel;
        
        if (vm is WorkflowViewModel workflowViewModel)
            WorkflowViewModel = workflowViewModel;

        CurrentPage = vm;
    }

    [RelayCommand]
    public void NavigateToWorkspace() => SetViewModel(new WorkspaceViewModel(LoadedWorkspace));
    
    [RelayCommand]
    public void NavigateToWorkflow() => SetViewModel(new WorkflowViewModel(LoadedWorkspace));

    [RelayCommand]
    public void NavigateToSettings() => CurrentPage = new SettingsViewModel();
}