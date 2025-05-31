using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Seederly.Desktop.Models;
using Seederly.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Seederly.Desktop.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public ObservableCollection<KeyValuePair<string, StagingEnvironmentModel>> StagingEnvironments { get; set; }

    [ObservableProperty] private KeyValuePair<string, StagingEnvironmentModel>? _selectedEnvironmentPair;

    public StagingEnvironmentModel? SelectedEnvironment => SelectedEnvironmentPair?.Value;

    public SettingsViewModel()
    {
        StagingEnvironments = new ObservableCollection<KeyValuePair<string, StagingEnvironmentModel>>(
            SessionService.Instance.LoadedWorkspace.StagingEnvironments
                .Select(se => new KeyValuePair<string, StagingEnvironmentModel>(
                    se.Key,
                    StagingEnvironmentModel.FromEnvironment(se.Value)
                )));
        
        // Select the first one by default
        SelectedEnvironmentPair = StagingEnvironments.FirstOrDefault();
    }

    [RelayCommand]
    private void SaveChanges()
    {
        if (SelectedEnvironment is null) return;
        
        // Parse the environments
        var updatedEnvironment = SelectedEnvironment.ToEnvironment();
        
        // Update the environment in the session
        var key = SelectedEnvironmentPair?.Value.Name ?? string.Empty;
        SessionService.Instance.LoadedWorkspace.StagingEnvironments[key] = updatedEnvironment;
        
        // Save the session
        SessionService.Instance.SaveWorkspace(); 
    }
}