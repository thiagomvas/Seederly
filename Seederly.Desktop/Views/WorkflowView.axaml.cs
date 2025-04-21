using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Seederly.Desktop.Models;
using Seederly.Desktop.ViewModels;

namespace Seederly.Desktop.Views;

public partial class WorkflowView : UserControl
{
    public WorkflowView()
    {
        InitializeComponent();
    }

    
    private void NameTextBlock_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is TextBlock tb && tb.DataContext is WorkflowModel vm)
            vm.IsEditing = true;
    }

    private void NameTextBox_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb && tb.DataContext is WorkflowModel vm)
            vm.IsEditing = false;
    }

    private void WorkflowStepList_Selected(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.DataContext is WorkflowViewModel vm)
        {
            var selectedStep = listBox.SelectedItem as WorkflowStepModel;
            if (selectedStep != null)
            {
                selectedStep.IsEditing = true;
            }
            foreach (var item in listBox.Items.OfType<WorkflowStepModel>())
            {
                if (item != selectedStep)
                {
                    item.IsEditing = false;
                }
            }
        }
    }
}