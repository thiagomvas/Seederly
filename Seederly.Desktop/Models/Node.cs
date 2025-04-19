using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Seederly.Desktop.Models;


public partial class Node<T> : ObservableObject
{
    public ObservableCollection<Node<T>> SubNodes { get; } = new();
    public Node<T>? Parent { get; set; }
    [ObservableProperty]
    private string _name;
    public bool IsLeaf => SubNodes.Count == 0 && Value is not null;
    public T? Value { get; set; }

    [ObservableProperty]
    private bool _isEditing;
        
    public Node(string name)
    {
        Name = name;
        SubNodes = new ObservableCollection<Node<T>>();
    }
        
    public Node(string name, ObservableCollection<Node<T>> subNodes)
    {
        Name = name;
        SubNodes = subNodes;
    }
    public Node(string name, T value)
    {
        Name = name;
        Value = value;
        SubNodes = new ObservableCollection<Node<T>>();
    }
    
    public void CreateNewChild()
    {
        var newNode = new Node<T>("New Node") {Parent = this};
        SubNodes.Add(newNode);
    }
}