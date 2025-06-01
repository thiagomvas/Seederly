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
    public ObservableCollection<StagingEnvironmentModel> Environments { get; set; }

    [ObservableProperty] private StagingEnvironmentModel _selectedEnv;


    public SettingsViewModel()
    {
        Environments = new ObservableCollection<StagingEnvironmentModel>(
            SessionService.Instance.LoadedWorkspace.EnvironmentList
                .Select(se => StagingEnvironmentModel.FromEnvironment(se))
        );
        SelectedEnv = Environments.FirstOrDefault() ?? new StagingEnvironmentModel
        {
            Name = "Production",
        };

        // Select the first one by default
    }
    
    [RelayCommand]
    private void AddEnvironment()
    {
        // Create a new environment model
        var newEnvironment = new StagingEnvironmentModel
        {
            Name = "New Environment",
        };
        
        // Add it to the collection
        Environments.Add(newEnvironment);
        
        // Select the newly added environment
        SelectedEnv = newEnvironment;
    }
    
    [RelayCommand]
    private void RemoveEnvironment()
    {
        if (SelectedEnv is null) return;
        
        // Remove the environment from the session
        var env = SessionService.Instance.LoadedWorkspace.EnvironmentList
            .FirstOrDefault(e => e.Name == SelectedEnv.Name);
        
        // Check if were removing the active environment
        if (env is not null)
        {
            if(env == SessionService.Instance.LoadedWorkspace.ActiveEnvironment)
            {
                SessionService.Instance.LoadedWorkspace.ActiveEnvironmentIndex--;
                
                if (SessionService.Instance.LoadedWorkspace.ActiveEnvironmentIndex < 0)
                    SessionService.Instance.LoadedWorkspace.ActiveEnvironmentIndex = 0;
            }
            SessionService.Instance.LoadedWorkspace.EnvironmentList.Remove(env);
        }
        
        // Remove the selected environment from the collection
        Environments.Remove(SelectedEnv);
        
        
        // Clear the selection if the collection is empty
        if (Environments.Count == 0)
        {
            SelectedEnv = new StagingEnvironmentModel
            {
                Name = "Production",
            };
            
            Environments.Add(SelectedEnv);
        }
        else
        {
            SelectedEnv = Environments.First();
        }
        
        SaveChanges();
    }
    
    [RelayCommand]
    private void ActivateEnvironment()
    {
        if (SelectedEnv is null) return;
        
        // Set the active environment in the session
        SessionService.Instance.LoadedWorkspace.ActiveEnvironmentIndex = Environments.IndexOf(SelectedEnv);
        if (MainWindowViewModel.Instance is not null)
            MainWindowViewModel.Instance.EnvironmentName = SelectedEnv.Name;
        
        // Save the session
        SessionService.Instance.SaveWorkspace();
    }

    [RelayCommand]
    private void SaveChanges()
    {
        // Parse the environments
        var envs = Environments.Select(e => e.ToEnvironment()).ToList();
        
        SessionService.Instance.LoadedWorkspace.EnvironmentList = envs;
        SessionService.Instance.SaveWorkspace();
        
    }
}