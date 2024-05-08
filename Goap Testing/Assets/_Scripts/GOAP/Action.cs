using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public WorldState preCondition { get; private set; }
    public WorldState postCondition { get; private set; }

    int cost;
    public delegate int CostDelegate();
    CostDelegate costDelegate;

    string name;

    // These will be used later to call the methods associated with the action
    public delegate void ActionDelegate();
    ActionDelegate actionDelegate;

    public Action(string name, int cost, WorldState preCondition, WorldState postCondition)
    {
        this.cost = cost;
        this.name = name;
        this.preCondition = preCondition;
        this.postCondition = postCondition;
    }
    public Action(string name, int cost, WorldState preCondition, WorldState postCondition, ActionDelegate action)
    {
        this.cost = cost;
        this.name = name;
        this.preCondition = preCondition;
        this.postCondition = postCondition;
        actionDelegate = action;
    }
    public Action(string name, CostDelegate cost, WorldState preCondition, WorldState postCondition, ActionDelegate action)
    {
        this.cost = -1;
        this.name = name;
        this.preCondition = preCondition;
        this.postCondition = postCondition;
        actionDelegate = action;
        costDelegate = cost;
    }

    /// <summary>
    /// Returns the value of the key within the preCondition dictionary
    /// </summary>
    /// <param name="key">The key which is being checked for</param>
    /// <returns>The value of the given key if the key is present, otherwise returns null</returns>
    public Property.Value Requires(Property.Key key)
    {
        if(preCondition.ContainsKey(key))
        {
            return preCondition.GetProperty(key);
        }
        return null;
    }
    /// <summary>
    /// Returns the value of the key within the postCondition dictionary
    /// </summary>
    /// <param name="key">The key which is being checked for</param>
    /// <returns>The value of the given key if the key is present, otherwise returns null</returns>
    public Property.Value Produces(Property.Key key)
    {
        if (postCondition.ContainsKey(key))
        {
            return postCondition.GetProperty(key);
        }
        return null;
    }

    /// <summary>
    /// Returns the cost of the action
    /// </summary>
    /// <returns>I'll give you one guess as to what it does, go ahead...</returns>
    public int GetCost()
    {
        if (costDelegate != null)
            return costDelegate();
        return cost;
    }
    /// <summary>
    /// Checks whether the action could be completed within the given state
    /// </summary>
    /// <param name="state">The state in which the action would be performed</param>
    /// <returns>True if the given state satisfies the action's preconditions, otherwise returns False</returns>
    public bool Doable(WorldState state)
    {
        return state.Satisfies(preCondition);
    }

    /// <summary>
    /// Performs the action that was passed into the actionDelegate variable
    /// </summary>
    /// <returns>True if the action has been completed, otherwise False</returns>
    public void DoAction()
    {
        actionDelegate();
    }

    public override string ToString()
    {
        return name;
    }
}
