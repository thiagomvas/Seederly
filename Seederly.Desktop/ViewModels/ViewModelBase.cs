using CommunityToolkit.Mvvm.ComponentModel;

namespace Seederly.Desktop.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public virtual void Dispose() { }
}