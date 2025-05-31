using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seederly.Core;
using Seederly.Core.Automation;
using Seederly.Desktop.Services;

namespace Seederly.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public static MainWindowViewModel? Instance { get; private set; }
    
    public LoggerService LoggerService { get; } = LoggerService.Instance;
    
    [ObservableProperty] private string _workspaceName = "New Workspace";
    [ObservableProperty] private string _environmentName = "Production";
    [ObservableProperty] private string _lastOperation = "No operations performed yet.";
    [ObservableProperty] private string _status = "Ready";
    [ObservableProperty] private bool _showLogs = false;

    [ObservableProperty]
    private ViewModelBase? currentPage;
    
    [ObservableProperty]
    private WorkspaceViewModel _workspaceViewModel ;
    
    [ObservableProperty]
    private WorkflowViewModel _workflowViewModel;

    public MainWindowViewModel()
    {
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
    public void NavigateToWorkspace() => SetViewModel(new WorkspaceViewModel(SessionService.Instance.LoadedWorkspace));
    
    [RelayCommand]
    public void NavigateToWorkflow() => SetViewModel(new WorkflowViewModel(SessionService.Instance.LoadedWorkspace));

    [RelayCommand]
    public void NavigateToSettings() => CurrentPage = new SettingsViewModel();
    
    [RelayCommand] 
    private void ToggleLogs() => ShowLogs = !ShowLogs;
    [RelayCommand]
    private void ClearLogs() => LoggerService.ClearLogs();

    [RelayCommand]
    private async Task ExportLogs()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Export Logs",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Text Files", Extensions = { "txt" } },
                new FileDialogFilter { Name = "All Files", Extensions = { "*" } }
            },
            DefaultExtension = "txt"
        };

        var window = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (window is null)
            return;

        var filePath = await dialog.ShowAsync(window);
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var sb = new StringBuilder();
        foreach (var entry in LoggerService.LogEntries)
        {
            sb.AppendLine($"[{entry.Level}] [{entry.Timestamp}] {entry.Message}");
        }

        await File.WriteAllTextAsync(filePath, sb.ToString());
    }

}