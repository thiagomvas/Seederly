using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Seederly.Desktop.Models;

namespace Seederly.Desktop.Views;

public partial class WorkspaceView : UserControl
{
    public ObservableCollection<string> AvailableDataTypes { get; } = new()
    {
        "name.firstName", "name.lastName", "name.fullName"
    };

    public WorkspaceView()
    {
        InitializeComponent();
    }

    private void NameTextBlock_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is TextBlock tb && tb.DataContext is Node<ApiEndpointModel> vm)
            vm.IsEditing = true;
    }

    private void NameTextBox_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb && tb.DataContext is Node<ApiEndpointModel> vm)
            vm.IsEditing = false;
    }
}