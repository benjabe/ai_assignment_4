using System.Collections.Generic;
using UnityEngine;

public static class GoapPlanner
{
    private static bool _debugEnabled = false;

    /// <summary>
    /// Creates an action plan based on a set of inital conditions
    /// and available actions
    /// </summary>
    /// <param name="initialConditions">
    /// The initial world state to use for the planning.
    /// </param>
    /// <param name="goal">
    /// The conditions that must be met at the end of the plan execution.
    /// </param>
    /// <param name="actions">
    /// The actions that are available for use in the planning.
    /// </param>
    /// <returns>
    /// A list of GoapActions ready to be performed.
    /// </returns>
    public static List<GoapAction> CreatePlan(
        Dictionary<string, object> initialConditions,
        Dictionary<string, object> goal,
        GoapAction[] actions)
    {
        // Actions which are possible given the initial conditions
        HashSet<GoapAction> usableActions = new HashSet<GoapAction>();

        // Reset all actions and get the
        // actions which are possible to
        // perform with the inital conditions
        foreach (GoapAction action in actions)
        {
            action.DoReset();
            /*
            foreach (var precondition in action.Preconditions)
            {
                if (initialConditions.ContainsKey(precondition.Key) &&
                    initialConditions[precondition.Key]
                        .Equals(precondition.Value))
                {
                    usableActions.Add(action);
                }
            }
            */
            usableActions.Add(action);
        }

        // Create a tree of possible plans.
        // Leaves represent the first action in a plan.
        List<GoapNode> leaves = new List<GoapNode>();

        // Build the planning tree.
        GoapNode start = new GoapNode(null, 0, initialConditions, null);
        bool success = CreatePlanningTree(start, leaves, usableActions, goal);
        
        if (success)
        {
            // We successfully built a planning tree, now we need to
            // find the cheapest plan
            GoapNode cheapest = null;
            foreach (GoapNode leaf in leaves)
            {
                if (cheapest == null ||
                    leaf.RunningCost < cheapest.RunningCost)
                {
                    cheapest = leaf;
                }
            }

            // Work back through the leaf's parents and reconstruct the path
            List<GoapAction> plan = new List<GoapAction>();
            GoapNode currentNode = cheapest;
            while (currentNode != null)
            {
                if (currentNode.Action != null)
                {
                    plan.Insert(0, currentNode.Action);
                }
                currentNode = currentNode.Parent;
            }

            return plan;
        }
        else
        {
            // No plan
            return null;
        }
    }

    /// <summary>
    /// Creates a goap planning tree.
    /// </summary>
    /// <param name="parent">The node we're working down from.</param>
    /// <param name="leaves"></param>
    /// <param name="usableActions"></param>
    /// <param name="Dictionary<string"></param>
    /// <param name="goalState"></param>
    /// <returns>True if success, false otherwise.</returns>
    private static bool CreatePlanningTree(
        GoapNode parent,
        List<GoapNode> leaves,
        HashSet<GoapAction> usableActions,
        Dictionary<string, object> goalState)
    {
        bool success = false;

        // Go through each action and see if it can be used
        foreach (GoapAction action in usableActions)
        {
            if (_debugEnabled)
            {
                // Check if the action us usable given the parent's state
                if (parent.Action != null)
                {
                    Debug.Log(parent.Action.GetType().Name);
                }
                else
                {
                    Debug.Log("No parent action.");
                }
                Debug.Log("\t->" + action.GetType().Name);
            }
            bool canUseAction = true;
            foreach (var precondition in action.Preconditions)
            {
                if (_debugEnabled)
                {
                    Debug.Log("\t\t" + precondition.Key + " must be " +
                            precondition.Value);
                }
                if (parent.State.ContainsKey(precondition.Key))
                {
                    object parentValue = parent.State[precondition.Key];
                    if (!parentValue.Equals(precondition.Value))
                    {
                        canUseAction = false;
                    }
                    if (_debugEnabled)
                    {
                        Debug.Log("\t\t" + precondition.Key +
                                " was " + parentValue);
                    }
                }
                else
                {
                    if (_debugEnabled)
                    {
                        Debug.Log(
                            "\t\t" + precondition.Key + " was not present."
                        );
                    }
                    canUseAction = false;
                }
            }

            if (_debugEnabled)
            {
                Debug.Log("\t\tCan use " + canUseAction);
            }

            if (canUseAction)
            {
                // Set current state to be
                // parent's state + the action's effects
                var currentState = new Dictionary<string, object>();
                foreach (var key in parent.State.Keys)
                {
                    currentState[key] = parent.State[key];
                }
                foreach (var effect in action.Effects)
                {
                    currentState[effect.Key] = effect.Value;
                    if (_debugEnabled)
                    {
                        Debug.Log("\t\tEffect: " + effect.Key + " " +
                                currentState[effect.Key]);
                    }
                }

                GoapNode node = new GoapNode(
                    parent,
                    parent.RunningCost + action.Cost,
                    currentState,
                    action
                );

                bool isDone = true;
                foreach (var goal in goalState)
                {
                    if (currentState.ContainsKey(goal.Key))
                    {
                        if (!currentState[goal.Key].Equals(goal.Value))
                        {
                            isDone = false;
                        }
                    }
                    else
                    {
                        isDone = false;
                    }
                }

                if (isDone)
                {
                    if (_debugEnabled)
                    {
                        Debug.Log("--------- Done! ---------");
                    }
                    leaves.Add(node);
                    success = true;
                }
                else
                {
                    var subsetActions = new HashSet<GoapAction>();
                    if (_debugEnabled)
                    {
                        Debug.Log("\t\t\tSubset actions");
                    }
                    foreach (var subsetAction in usableActions)
                    {
                        if (subsetAction != action)
                        {
                            subsetActions.Add(subsetAction);
                            if (_debugEnabled)
                            {
                                Debug.Log("\t\t\t\tAdded " +
                                        subsetAction.GetType().Name);
                            }
                        }
                    }
                    if (_debugEnabled)
                    {
                        foreach (var subsetAction in subsetActions)
                        {
                            Debug.Log("\t\t\t\tContains " +
                                subsetAction.GetType().Name);
                        }
                        Debug.Log("\t\t\tCreate subset");
                    }
                    bool subsetSuccess = CreatePlanningTree(
                        node,
                        leaves,
                        subsetActions,
                        goalState
                    );
                    if (_debugEnabled)
                    {
                        Debug.Log("\t\t\t\t\t\t" + subsetSuccess);
                    }
                    if (subsetSuccess)
                    {
                        success = true;
                    }
                }
            }
        }

        return success;
    }

    /// <summary>
    /// Checks whether an action is possible given a set of conditions.
    /// </summary>
    /// <param name="action">The action to be checked.</param>
    /// <param name="conditions">The conditions to check against.</param>
    /// <returns>True if all preconditions are met.</returns>
    private static bool IsActionPossible(
        GoapAction action,
        Dictionary<string, object> conditions)
    {
        foreach (var precondition in action.Preconditions)
        {
            if (conditions.ContainsKey(precondition.Key))
            {
                if (!conditions[precondition.Key].Equals(precondition.Value))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Contains data about a node used to do GOAP.
    /// </summary>
    private class GoapNode
    {
        public GoapNode Parent;
        public float RunningCost;
        public Dictionary<string, object> State;
        public GoapAction Action;

        public GoapNode(
            GoapNode parent,
            float runningCost,
            Dictionary<string, object> state,
            GoapAction action)
        {
            Parent = parent;
            RunningCost = runningCost;
            State = state;
            Action = action;
        }
    }
}
