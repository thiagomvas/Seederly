using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
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
            var vm = new WorkspaceViewModel(files[0].Path.LocalPath);
            _viewModel.SetViewModel(vm);
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
                await using var stream = await file.OpenWriteAsync();
                using var streamWriter = new StreamWriter(stream);
                await streamWriter.WriteLineAsync(_viewModel.WorkspaceViewModel.SerializeWorkspace());
            }
        }
        else
        {
            // Open writing stream from the file.
            await using var stream = File.OpenWrite(_viewModel.WorkspaceViewModel.WorkspacePath);
            using var streamWriter = new StreamWriter(stream);
            await streamWriter.WriteLineAsync(_viewModel.WorkspaceViewModel.SerializeWorkspace());
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
            using var stream = file.OpenWriteAsync().Result;
            using var streamWriter = new StreamWriter(stream);
            streamWriter.WriteLine(_viewModel.WorkspaceViewModel.SerializeWorkspace());
        }
    }

    private async void ReportBugButton_Clicked(object? sender, RoutedEventArgs e) => await TopLevel.GetTopLevel(this).Launcher
        .LaunchUriAsync(new Uri("https://github.com/thiagomvas/Seederly/issues"));

    private void ExitButton_Clicked(object? sender, RoutedEventArgs e) => Environment.Exit(0);
}