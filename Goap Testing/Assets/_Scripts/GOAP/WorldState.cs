using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Property;

public class WorldState
{

    public Dictionary<Property.Key, Property.Value> properties { get; private set; } = new Dictionary<Property.Key, Property.Value>(new Property.Key.EqualityComparer());

    public WorldState(Dictionary<Property.Key, Property.Value> properties)
    {
        // Copies the data and adds the necessary compararer
        // This is needed to compare the Property.Key custom data type
        this.properties = new Dictionary<Property.Key, Property.Value>(properties, new Property.Key.EqualityComparer());
    }
    public WorldState()
    {
        properties = new Dictionary<Property.Key, Property.Value>(new Property.Key.EqualityComparer());
    }


    #region Basic Function Methods

    /// <summary>
    /// Returns a duplicate of this WorldState
    /// </summary>
    /// <returns>Returns a duplicate of this WorldState</returns>
    public WorldState Duplicate()
    {
        return new WorldState(new Dictionary<Property.Key, Property.Value>(properties, new Property.Key.EqualityComparer()));
    }

    /// <summary>
    /// Returns the size of the properties dictionary.
    /// </summary>
    /// <returns>Returns the size of the properties dictionary</returns>
    public int Count()
    {
        return properties.Count;
    }

    /// <summary>
    /// Returns whether the properties dictionary contains the given Property.Key
    /// </summary>
    /// <param name="key">The key that is being checked for</param>
    /// <returns>Returns true if the key is present within the properties dictionary</returns>
    public bool ContainsKey(Property.Key key)
    {
        return properties.ContainsKey(key);
    }

    /// <summary>
    /// Adds the given Key-Value-Pair to the properties dictionary. Overwrites data if key is already present.
    /// </summary>
    /// <param name="key">The key which will be added</param>
    /// <param name="value">The property which will be added</param>
    public void AddProperty(Property.Key key, Property.Value value)
    {
        if(!ContainsKey(key))
            properties.Add(key, value);
        else
            properties[key] = value;
    }

    /// <summary>
    /// Returns the property with the given key
    /// </summary>
    /// <param name="key">The key of the property to return</param>
    /// <returns>Returns a Property.Value if the key is valid, otherwise returns null</returns>
    public Property.Value GetProperty(Property.Key key)
    {
        if(ContainsKey(key))
            return properties[key];
        return null;
    }

    /// <summary>
    /// Removes a property from the dictionary if the key is present
    /// </summary>
    /// <param name="key">The key of the property to remove</param>
    public void DropProperty(Property.Key key)
    {
        if(ContainsKey(key))
        {
            properties.Remove(key);
        }
    }

    /// <summary>
    /// Changes the property with the given key based on the given properties change type
    /// </summary>
    /// <param name="key">The key of the property to change</param>
    /// <param name="value">The new property value to update with. Should contain a Property.Value.MergeType, aka should be a postcondition</param>
    public void ChangeProperty(Property.Key key, Property.Value value)
    {
        if (ContainsKey(key))
        {
            properties[key] = value.Unify(properties[key]);
        }
    }

    #endregion

    #region Advanced Functions

    /// <summary>
    /// Combines two WorldStates into the WorldState that called the function
    /// </summary>
    /// <param name="other">The WorldState which is being merged.</param>
    /// <param name="overwrite">Should values in the original WorldState be overwritten. Default is True.</param>
    public void Merge(WorldState other, bool overwrite = true)
    {
        foreach (Property.Key key in other.properties.Keys)
        {
            if (!properties.ContainsKey(key))
                properties.Add(key, other.properties[key]);
            else if (overwrite)
            {
                switch (other.GetProperty(key).mergeType)
                {
                    case Value.MergeType.ADD:
                        properties[key] += other.properties[key];
                        break;
                    case Value.MergeType.MULTIPLY:
                        properties[key] *= other.properties[key];
                        break;
                    case Value.MergeType.SET:
                        properties[key] = other.properties[key];
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Checks whether all of the properties within the other WorldState are present and equvilant in the calling WorldState
    /// </summary>
    /// <param name="other">The WorldState that is being evaluated to be within the calling WorldState</param>
    /// <returns>
    /// True if the other WorldState does not contain any keys that are either not present within the calling WorldState
    /// and if all of the values of these keys are the same between both WorldStates, otherwise returns False.
    /// </returns>
    public bool Satisfies(WorldState other)
    {
        // Loop through all the properties within the compared state
        foreach(Key key in other.properties.Keys)
        {
            // If the calling state contains the given key and that value does not meet the requirements, then the satisfy check fails
            if (properties.ContainsKey(key) && !other.GetProperty(key).CompareAgainst(GetProperty(key)))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Applies the action's post conditions to the current WorldState if the action is doable
    /// </summary>
    /// <param name="action">The action to be performed</param>
    /// <returns>True if the action is doable and is applied, otherwise False</returns>
    public bool Apply(Action action)
    {
        if (action.Doable(this))
        {
            Merge(action.postCondition);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Evaluates whether the two states are equvilant
    /// </summary>
    /// <param name="state">The state to check against the calling state</param>
    /// <returns>True if the states are equivilant, otherwise False</returns>
    public bool Equals(WorldState state)
    {
        foreach(Property.Key key in state.properties.Keys)
        {
            if (!ContainsKey(key) || !GetProperty(key).Equals(state.GetProperty(key)))
                return false;
        }
        foreach (Property.Key key in properties.Keys)
        {
            if (!state.ContainsKey(key) || !GetProperty(key).Equals(state.GetProperty(key)))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Combines one worldstate into the other on the basis of ownership of the property. Only values which are owned by the other WorldState will be overwritten.
    /// </summary>
    /// <param name="other">The WorldState to combine into the calling state</param>
    public void Combine(WorldState other, GameObject owner)
    {
        // Run through all the properties coming from the sender
        foreach(Property.Key key in other.properties.Keys)
        {
            // If the sender is the owner of that property, change this WorldState's property to reflect the new value
            if(key.GetSubject() == null || key.GetSubject() == owner)
                properties[key] = other.GetProperty(key);
            else if(!properties.ContainsKey(key))
                properties.Add(key, other.GetProperty(key));
        }
    }

    /// <summary>
    /// This function compares the post unification WorldState to check if there has been any positive progress at all towards the goal state
    /// </summary>
    /// <param name="prior">The state prior to the unified state</param>
    /// <param name="current">The state that is being worked toward</param>
    /// <returns>True if progress has been made, otherwise False</returns>
    public bool ProgressCompare(WorldState prior, WorldState current)
    {
        foreach(Key key in prior.properties.Keys)
        {
            if (ContainsKey(key))
            {
                if(!current.ContainsKey(key) || !prior.GetProperty(key).CompareAgainst(current.GetProperty(key)))
                {
                    if(GetProperty(key).compareType == Value.CompareType.EQUAL && !GetProperty(key).Equals(prior.GetProperty(key)))
                    {
                        return true;
                    }
                    else if (!GetProperty(key).Equals(prior.GetProperty(key)) && prior.GetProperty(key).CompareWith(GetProperty(key)))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Helper Functions

    public override string ToString()
    {
        string s = string.Empty;
        /*foreach(Property.Key prop in properties.Keys)
        {
            s += "\n" + prop + ": " + properties[prop];
        }*/
        foreach(Property.Key prop in properties.Keys)
        {
            s += "\n" + prop + ": " + properties[prop].data;
        }
        return s;
    }

    public class EqualityComparer : IEqualityComparer<WorldState>
    {
        public bool Equals(WorldState x, WorldState y)
        {
            foreach(Key key in x.properties.Keys)
            {
                if(!y.ContainsKey(key) || !y.GetProperty(key).Equals(x.GetProperty(key)))
                    return false;
            }
            return true;
        }
        public int GetHashCode(WorldState obj)
        {
            int hash = 0;
            foreach(Key key in obj.properties.Keys)
            {
                hash ^= key.GetHashCode();
                hash ^= obj.properties[key].GetHashCode();
            }
            return hash;
        }
    }

    #endregion
}
