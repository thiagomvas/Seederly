using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seederly.Core.Configuration;

namespace Seederly.Desktop.Models;

public partial class StagingEnvironmentModel : ObservableObject
{
    [ObservableProperty] private string _name = "Staging";
    [ObservableProperty] private string _baseUrl = "https://staging.api.seederly.com";
    [ObservableProperty] private string _documentationUrl = "https://staging.api.seederly.com";
    public ObservableCollection<HeaderEntry> Variables { get; set; } = new();
    
    [RelayCommand]
    public void AddVariable()
    {
        Variables.Add(new("", ""));
    }
    
    [RelayCommand]
    public void RemoveVariable(HeaderEntry variable)
    {
        Variables.Remove(variable);
    }
    public static StagingEnvironmentModel FromEnvironment(StagingEnvironment coreModel)
    {
        return new StagingEnvironmentModel
        {
            Name = coreModel.Name,
            BaseUrl = coreModel.BaseUrl,
            DocumentationUrl = coreModel.DocumentationUrl,
            Variables = new(coreModel.Variables.Select(v => new HeaderEntry(v.Key, v.Value)))
        };
    }
    
    public StagingEnvironment ToEnvironment()
    {
        return new StagingEnvironment
        {
            Name = this.Name,
            BaseUrl = this.BaseUrl,
            DocumentationUrl = this.DocumentationUrl,
            Variables = this.Variables.ToDictionary(v => v.Key, v => v.Value)
        };
    }
}