using System;
using System.Collections.Generic;
using UnityEngine;

public class GOAP
{
    
    /// <summary>
    /// Perform the actions listed in order in the plan, if an action cannot be performed, end the plan
    /// </summary>
    /// <param name="plan">The list of actions to be performed</param>
    /// <param name="state">The WorldState on which to perform the actions</param>
    /// <returns>True if all actions were performed successfully, otherwise False</returns>
    public static bool Execute(List<Action> plan, WorldState state)
    {
        foreach(Action action in plan)
        {
            if(action.Doable(state))
            {
                if(!state.Apply(action))
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// A modified version of the Levenshtein distance, checks the differences in properties between the two states
    /// </summary>
    /// <param name="src">The current WorldState</param>
    /// <param name="dst">The WorldState to check the distance to</param>
    /// <returns>The heuristic distance between the two WorldStates</returns>
    public static int Distance(WorldState src, WorldState dst) 
    {
        int dist = 0;
        foreach(Property.Key key in dst.properties.Keys)
        {
            if (!src.properties.ContainsKey(key))
            {
                dist++;
            }
            else
            {
                switch (Type.GetTypeCode(src.GetProperty(key).dataType))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        dist += Mathf.Abs((int)src.GetProperty(key).data - (int)dst.GetProperty(key).data);
                        break;
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        dist += (int)Mathf.Abs((float)src.GetProperty(key).data - (float)dst.GetProperty(key).data);
                        break;
                    case TypeCode.Boolean:
                        dist += src.GetProperty(key).Equals(dst.GetProperty(key)) ? 0 : 1;
                        break;
                }
            }
        }

        return dist;
    }

    /// <summary>
    /// Checks whether the dst WorldState contains a key that is also present within the src WorldState and if this key has the same value in both.
    /// </summary>
    /// <param name="src">The current WorldState</param>
    /// <param name="dst">The destination WorldState</param>
    /// <returns>True if all keys within the dst WorldState are either not present in, or are present in and have the same values as keys in the src WorldState, otherwise False</returns>
    public static bool Conflict(WorldState src, WorldState dst)
    {
        foreach (Property.Key key in dst.properties.Keys)
        {
            if (src.ContainsKey(key) && !src.GetProperty(key).Equals(dst.GetProperty(key)))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Combines the Goal State and the Action to give the WorldState that must exist before the action that led to the Goal State
    /// </summary>
    /// <param name="action">The action that is being checked</param>
    /// <param name="goalState">The GoalState that will be checked</param>
    /// <returns>The WorldState that needed to exist in order to perform the action to get to the GoalState</returns>
    public static WorldState Unify(Action action, WorldState goalState)
    {
        // Make a copy of the goal state so that we can return the orignial later if needed
        WorldState newState = goalState.Duplicate();

        // Loop through all properties in the action's postcondition
        // If the property conflicts with the current goal, exit
        // If the property appears in the goal and doesn't conflict, remove it
        foreach (Property.Key key in action.postCondition.properties.Keys)
        {
            if (newState.ContainsKey(key) && !newState.GetProperty(key).UnifyCompare(action.postCondition.GetProperty(key)))
                return null;

            else if (newState.ContainsKey(key))
            {
                if (action.postCondition.GetProperty(key).mergeType == Property.Value.MergeType.SET)
                    newState.DropProperty(key);
                else
                    newState.ChangeProperty(key, action.postCondition.GetProperty(key));
            }
        }

        // If the state hasn't / won't change, return null as this action doens't really do anything
        if (goalState.Equals(newState))
            return null;

        // Loop through all the properties in the action's precondition
        // If the property does not appear in the newState, add it
        // If the precondition's properties could not be fulfilled by this state, return null
        foreach (Property.Key key in action.preCondition.properties.Keys)
        {
            if (!newState.ContainsKey(key))
                newState.AddProperty(key, action.preCondition.GetProperty(key));
            else if (!action.preCondition.GetProperty(key).UnifyCompare2(newState.GetProperty(key)))
                return null;
        }

        //Debug.Log("Adding " + action);
        return newState;
    }

    /// <summary>
    /// Creates a plan to navigate from the given currentState to the given GoalState
    /// </summary>
    /// <param name="actions">The actions available to perform</param>
    /// <param name="currentState">The current WorldState</param>
    /// <param name="goalState">The desired WorldState</param>
    /// <returns>The List of Actions to get between the two WorldStates if there is one, if not returns an empty List of Actions</returns>
    public static List<Action> Search(List<Action> actions, WorldState currentState, WorldState goalState)
    {
        // This search constructs the path backwards from the goalState

        // Create the plan which will be used later
        List<Action> plan = new List<Action>();

        // The WorldState to be checked, iterates in the while loop
        WorldState currentGoal = goalState;

        // The queue that will store the WorldStates that we will look through
        PriorityQueue queue = new PriorityQueue();
        queue.Insert(new PriorityQueue.Element(currentGoal, 1));

        // The "path" to be traced back later when reconstructing the optimal path
        // Holds a WorldState as the key and SearchData as the Value
        // SearchData contains the action that was performed and the resulting end state that was reached
        Dictionary<WorldState, SearchData> cameFrom = new Dictionary<WorldState, SearchData>();

        // Contains the minimum cost that it has taken to get to a given WorldState
        Dictionary<WorldState, int> costSoFar = new Dictionary<WorldState, int>(new WorldState.EqualityComparer());

        cameFrom.Add(currentGoal, new SearchData(null, null));
        costSoFar.Add(currentGoal, 1);

        // A check to see if we have found a path or not
        bool pathFound = false;

        // Adds a cutoff to the amount of times it can run, stops a potential crash
        int ittLimit = 100000;
        int itteration = 0;

        while(!queue.Is_Empty())
        {
            // Get the lowest priority item from the queue
            //Debug.Log("Lowest Priority: " + queue.Front());
            currentGoal = (WorldState)queue.Extract();

            //Debug.Log("Current Goal --------------------------------------------------------------------------------------------\n" + currentGoal);
            //Debug.Log("Checking for Satisfies\nCurrent State:" + currentState + "\nCurrent Goal:" + currentGoal);

            // If the currentState is satisfied by the currentGoal, we have found our path and we can exit
            if (currentState.Satisfies(currentGoal) || itteration >= ittLimit)
            {
                Debug.Log("Path Found at " + itteration + " iterations\n" + currentGoal);
                pathFound = true;
                break;
            }

            itteration++;

            // Loop through all the possible actions
            foreach (Action act in actions)
            {
                // The outcome of the action being performed
                // Really represents the WorldState that exists before the goalState should this aciton be performed
                WorldState actionOutcome = Unify(act, currentGoal);
                if (actionOutcome == null)
                    continue;

                // The new cost for the action being performed
                int newCost = costSoFar[currentGoal] + act.GetCost();

                // Checks to see if there is either no cost for the outcome yet or if this is a more cost effective way to get there
                if (!costSoFar.ContainsKey(actionOutcome) || newCost < costSoFar[actionOutcome])
                {
                    // Sets the new cost of getting to the actionOutcome WorldState
                    costSoFar[actionOutcome] = newCost;

                    // Determines the priority of the WorldState
                    int priority = newCost + Distance(actionOutcome, currentState);

                    // Inserts this WorldState into the queue with the given priority to be checked later on
                    queue.Insert(new PriorityQueue.Element(actionOutcome, priority));

                    // Adds / Changes the cameFrom dictionary to allow for reconstruction later
                    if (cameFrom.ContainsKey(actionOutcome))
                        cameFrom[actionOutcome] = new SearchData(currentGoal, act);
                    else
                        cameFrom.Add(actionOutcome, new SearchData(currentGoal, act));
                }
            }
        }

        WorldState current = currentGoal;

        // Reconstructing the Path
        if (pathFound)
        {
            while (true)
            {
                plan.Add(cameFrom[current].action);
                current = cameFrom[current].state;
                if (current == goalState)
                    break;
            }

            WorldState tempState = goalState.Duplicate();

            Debug.Log("Start -> " + tempState);
            for (int i = plan.Count - 1; i >= 0; i--)
            {
                tempState = Unify(plan[i], tempState);
                //Debug.Log(plan[i] + " -> " + tempState);
            }

        }

        return plan;
    }
    class SearchData
    {
        public WorldState state;
        public Action action;

        public SearchData(WorldState state, Action action)
        {
            this.state = state;
            this.action = action;
        }
    }
}
