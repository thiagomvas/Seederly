using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
}