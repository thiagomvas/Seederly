using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Seederly.Core;
using Seederly.Core.Automation;
using Seederly.Core.OpenApi;
using Seederly.Desktop.Services;
using Seederly.Desktop.ViewModels;

namespace Seederly.Desktop.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? _viewModel;
    public MainWindow()
    {
        InitializeComponent();
        if (_viewModel is null)
            _viewModel = (MainWindowViewModel)DataContext!;
    }
    
    private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        if (_viewModel is null)
            _viewModel = (MainWindowViewModel)DataContext!;
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            _viewModel.LoadedWorkspace = Workspace.DeserializeFromJson(File.ReadAllText(files[0].Path.LocalPath));
            _viewModel.NavigateToWorkspace();
            SessionService.Instance.Data.LastWorkspacePath = files[0].Path.LocalPath;
            SessionService.Instance.SaveData();
        }
    }
    
    private async void SaveFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        if (_viewModel is null)
            _viewModel = (MainWindowViewModel)DataContext!;

        if (string.IsNullOrWhiteSpace(_viewModel.WorkspaceViewModel.WorkspacePath))
        {
            // Start async operation to open the dialog.
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Workspace"
            });

            if (file is not null)
            {
                // Open writing stream from the file.
                _viewModel.WorkspaceViewModel.WorkspacePath = file.Path.LocalPath;
                _viewModel.LoadedWorkspace.Path = file.Path.LocalPath;
                Utils.SaveWorkspace(_viewModel.LoadedWorkspace);
            }
        }
        else
        {
            Utils.SaveWorkspace(_viewModel.LoadedWorkspace);
        }
    }

    private async void SaveFileAsButton_Clicked(object? sender, RoutedEventArgs e)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        if (_viewModel is null)
            _viewModel = (MainWindowViewModel)DataContext!;

        // Start async operation to open the dialog.
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Workspace"
        });

        if (file is not null)
        {
            _viewModel.WorkspaceViewModel.WorkspacePath = file.Path.LocalPath;
            _viewModel.LoadedWorkspace.Path = file.Path.LocalPath;
            Utils.SaveWorkspace(_viewModel.LoadedWorkspace);
        }
    }

    private async void ReportBugButton_Clicked(object? sender, RoutedEventArgs e) => await TopLevel.GetTopLevel(this).Launcher
        .LaunchUriAsync(new Uri("https://github.com/thiagomvas/Seederly/issues"));

    private void ExitButton_Clicked(object? sender, RoutedEventArgs e) => Environment.Exit(0);

    private async void SendAllRequests_Clicked(object? sender, RoutedEventArgs e) =>
        await _viewModel?.WorkspaceViewModel?.ExecuteAllRequests();

    private async void ImportFromOpenApiButton_Clicked(object? sender, RoutedEventArgs e)
    {
        if (_viewModel is null)
            _viewModel = (MainWindowViewModel)DataContext!;
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            var json = File.ReadAllText(files[0].Path.LocalPath);
            var document = OpenApiDocument.FromReferenceJson(json);
            var workspace = Workspace.CreateFromOpenApiDocument(document);
            _viewModel.LoadedWorkspace = workspace;
            _viewModel.NavigateToWorkspace();
        }
    }

    private async void DocumentationButton_Clicked(object? sender, RoutedEventArgs e)=> await TopLevel.GetTopLevel(this).Launcher
        .LaunchUriAsync(new Uri("https://github.com/thiagomvas/Seederly/wiki"));
}