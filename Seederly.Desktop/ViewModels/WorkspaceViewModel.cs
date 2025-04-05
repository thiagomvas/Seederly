using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Seederly.Desktop.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{
    
    public ObservableCollection<Node> Nodes { get; } = new();

    public WorkspaceViewModel()
    {
        Nodes.Add(new Node("Users", new ObservableCollection<Node>
        {
            new Node("GET - Get Users"),
            new Node("POST - Create User"),
            new Node("PUT - Update User"),
            new Node("DELETE - Delete User")
        }));

        Nodes.Add(new Node("Products", new ObservableCollection<Node>
        {
            new Node("GET - Get Products"),
            new Node("POST - Create Product")
        }));
    }

    public class Node
    {
        public ObservableCollection<Node> SubNodes { get; }
        public string Name { get; }
        
        public Node(string name)
        {
            Name = name;
            SubNodes = new ObservableCollection<Node>();
        }
        
        public Node(string name, ObservableCollection<Node> subNodes)
        {
            Name = name;
            SubNodes = subNodes;
        }
    }
}