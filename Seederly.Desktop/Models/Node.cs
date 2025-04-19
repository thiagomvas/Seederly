using System.Collections.ObjectModel;

namespace Seederly.Desktop.Models;


public class Node<T>
{
    public ObservableCollection<Node<T>> SubNodes { get; }
    public string Name { get; }
    public bool IsLeaf => SubNodes.Count == 0;
    public T? Value { get; set; }
        
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
        var newNode = new Node<T>("New Node");
        SubNodes.Add(newNode);
    }
}