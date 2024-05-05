using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode
{
    public string nodeName;
    public List<TreeNode> Children = new List<TreeNode>();
    public int currentChild = 0;

    //This declares a variable which can only be set as those three values represeting the state of the node itself
    public enum Status { SUCCESS, RUNNING, FAILED };
    public Status currentStatus = Status.RUNNING;

    public TreeNode(string n)
    {
        //Initializes the node (Constructor)
        nodeName = n;
    }

    public void AddChild(TreeNode n)
    {
        //Adds a child represented by Node n
        Children.Add(n);
    }

    public bool RemoveChild(TreeNode n)
    {
        //Removes a child represented by Node n
        return Children.Remove(n);
    }

    //The virtual keyword in C# defines a method in a parent class a child can override.
    //This allows another class to use the method or defines its own version of the same method using the same return type, name, and parameters. 
    public virtual Status Check()
    {
        //Returns the current status state that the node is in
        return currentStatus;
    }
}
