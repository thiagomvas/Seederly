using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Seederly.Core.Codegen;

namespace Seederly.Desktop.Views;

public partial class ApiCodeGenerationView : UserControl
{
    public ApiCodeGenerationView()
    {
        InitializeComponent();

        LanguageComboBox.ItemsSource = Enum.GetValues<CodeLanguage>().Select(e => e.ToString());
        LanguageComboBox.SelectedIndex = 0;
    }
    
    private async void CopyButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this).Clipboard;
        if (clipboard == null) return;
        
        var code = GeneratedCodeTextBox.Text;
        if (!string.IsNullOrEmpty(code))
        {
            await clipboard.SetTextAsync(code);
        }
    }
}