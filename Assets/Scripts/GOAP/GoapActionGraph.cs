using System.Collections.Generic;
using UnityEngine;

public class GoapActionGraph
{
    /// <summary>
    /// Maps GoapActions to their corresponding node.
    /// </summary>
    private Dictionary<GoapAction, Node<GoapAction>> _nodes = null;

    /// <summary>
    /// Constructs a GoapActionGraph from a list of GoapActions.
    /// </summary>
    /// <param name="GoapAction">
    /// The actions from which to construct the graph.
    /// </param>
    public GoapActionGraph(GoapAction[] actions)
    {
        _nodes = new Dictionary<GoapAction, Node<GoapAction>>();
        // For each action we create edges to any actions
        // whose postconditions may cause the action's preconditions
        // to be met.
        // UPDATE: Well that didn't work, since an action might need to rely
        // on two completely unrelated actions. If a path can't be created through
        // those no plan can be made. The solution is to create an edge from
        // each action to each action.

        // Populate the node dictionary.
        foreach (GoapAction action in actions)
        {
            _nodes[action] = new Node<GoapAction>();
            _nodes[action].Data = action;
        }

        // Create edges between nodes.
        // fromAction is the action from which an edge connects
        // toAction is the action to which an edge connects
        foreach (GoapAction fromAction in actions)
        {
            foreach (GoapAction toAction in actions)
            {
                _nodes[fromAction].Edges.Add(
                    new Edge<GoapAction>(
                        toAction.Cost,
                        _nodes[toAction]
                    )
                );
                /*
                // Use this to ensure that we only create one edge
                // between fromAction and toAction
                bool edgeCreated = false;
                foreach (var precondition in fromAction.Preconditions)
                {
                    Debug.Log("precondition: " + precondition.Key + " " +
                              precondition.Value);
                    foreach (var effect in toAction.Effects)
                    {
                        Debug.Log("\teffect: " + effect.Key + " " +
                                  effect.Value);
                        Debug.Log("\t\tkey: " +
                                  (precondition.Key.Equals(effect.Key)));
                        Debug.Log("\t\tvalue: " +
                                  (precondition.Value.Equals(effect.Value)));
                        if (precondition.Key == effect.Key &&
                            precondition.Value.Equals(effect.Value))
                        {
                            //Debug.Log("Edge created");
                            _nodes[fromAction].Edges.Add(
                                new Edge<GoapAction>(
                                    toAction.Cost,
                                    _nodes[toAction]
                                )
                            );
                            edgeCreated = true;
                            break;
                        }
                    }
                    //Debug.Log(" ");
                    if (edgeCreated)
                    {
                        break;
                    }
                }
                */
            }
        }

        // -- Debug --
        /*
        Debug.Log("Goap Action Graph");
        foreach (var node in _nodes.Values)
        {
            Debug.Log("\t" + node.Data.GetType().Name);
            foreach (var edge in node.Edges)
            {
                Debug.Log("\t->" + edge.Node.Data.GetType().Name);
            }
        }
        */
    }

    /// <summary>
    /// Creates a Goap action plan based on a desired change in world state.
    /// </summary>
    /// <param name="goal">The desired effect of the plan.</param>
    /// <returns>The list of GoapAction nodes in the plan.</returns>
    public List<Node<GoapAction>> CreatePlan(
        Dictionary<string, object> initialConditions,
        KeyValuePair<string, object> goal)
    {
        // Actions which would lead to the goal being met
        List<GoapAction> goalActions = new List<GoapAction>();

        // Populate startActions and goalActions
        foreach (GoapAction action in _nodes.Keys)
        {
            // Populate the goalActions list
            foreach (var kvp in action.Effects)
            {
                if (kvp.Key == goal.Key)
                {
                    goalActions.Add(action);
                }
            }
        }

        // Try to create a path.
        foreach (GoapAction goalAction in goalActions)
        {
            List<Node<GoapAction>> path = ConstructPathToAction(
                initialConditions,
                _nodes[goalAction]
            );

            if (path != null)
            {
                return path;
            }
        }

        // Could not find a suitable plan.
        return null;
    }


    /// <summary>
    /// Constructs a path to an action starting with an action which respects
    /// the initial preconditions.
    /// </summary>
    /// <param name="initialConditions">The initial conditions.</param<
    /// <param name="goal">The last action of the plan.</param>
    /// <returns>The list of GoapAction nodes in the path.</returns>
    private List<Node<GoapAction>> ConstructPathToAction(
        Dictionary<string, object> initialConditions,
        Node<GoapAction> goal)
    {
        // List of nodes that do not need to be (re-)expanded.
        List<Node<GoapAction>> closedSet = new List<Node<GoapAction>>();

        // List of discovered nodes that need to be (re-)expanded
        List<Node<GoapAction>> openSet = new List<Node<GoapAction>>();
        openSet.Add(goal);

        // Form node n cameFrom[n] returns the node
        // immediately preceding it on the cheapest known path
        Dictionary<Node<GoapAction>, Node<GoapAction>> cameFrom
            = new Dictionary<Node<GoapAction>, Node<GoapAction>>();

        // For node n, gScore[n] returns the cost of the cheapest known path.
        Dictionary<Node<GoapAction>, float> gScore =
            new Dictionary<Node<GoapAction>, float>();
        gScore.Add(goal, 0.0f);

        // For node n, fScore[n] = gScore[n] + heuristic cost(n)
        Dictionary<Node<GoapAction>, float> fScore =
            new Dictionary<Node<GoapAction>, float>();
        fScore.Add(goal, 0.0f);

        while (openSet.Count > 0)
        {
            // The node in openSet with the lowest fScore
            Node<GoapAction> current = openSet[0];

            foreach (Node<GoapAction> node in openSet)
            {
                if (fScore[node] < fScore[current])
                {
                    current = node;
                }
            }
            // Construct conditions arriving from current path
            // excluding goal because it might invalidate itself otherwise.
            var conditions = new Dictionary<string, object>(initialConditions);
            if (current != goal)
            {
                var currentCameFrom = cameFrom[current];
                Debug.Log(current.Data.GetType().Name + "->" +
                    currentCameFrom.Data.GetType().Name);
                foreach (var effect in current.Data.Effects)
                {
                    conditions[effect.Key] = effect.Value;
                }
                while (currentCameFrom != goal)
                {
                    foreach (var effect in currentCameFrom.Data.Effects)
                    {
                        conditions[effect.Key] = effect.Value;
                    }
                    currentCameFrom = cameFrom[currentCameFrom];
                }
                foreach (var condition in conditions)
                {
                    Debug.Log(condition.Key + " " + condition.Value);
                }
            }

            if (conditions.Count > 0 && IsActionPossible(goal.Data, conditions))
            {
                // Reconstruct path
                List<Node<GoapAction>> path = new List<Node<GoapAction>>();
                path.Add(current);

                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Add(current);
                }

                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Edge<GoapAction> edge in current.Edges)
            {
                Node<GoapAction> neighbor = edge.Node;
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + edge.Cost;

                if (!gScore.ContainsKey(neighbor) ||
                    tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    float heuristic = (float)EffectsMatchingPreconditions(
                        neighbor.Data,
                        goal.Data
                    );
                    fScore[neighbor] = gScore[neighbor] + heuristic;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // No path could be created.
        return null;
    }

    /// <summary>
    /// Checks if an action is possible given some conditions.
    /// </summary>
    /// <param name="action">
    /// The action whose possibility is in question.
    /// </param>
    /// <param name="conditions">
    /// The conditions against which we are testing the action.
    /// </param>.
    /// <returns>
    /// Returns true if the action is possible, false otherwise.
    /// </returns>
    private bool IsActionPossible(
        GoapAction action,
        Dictionary<string, object> conditions)
    {
        // For each KeyValuePair in the action's preconditions
        // Check if any condition contradicts the kvp's condition
        // or if the key is not present
        foreach (var precondition in action.Preconditions)
        {
            if (conditions.ContainsKey(precondition.Key))
            {
                // The condition set has en entry for this condition
                // Check if its value contradicts that of the precondition
                if (!conditions[precondition.Key].Equals(precondition.Value))
                {
                    // Contradiction found.
                    return false;
                }
            }
            else
            {
                // The condition was not met.
                return false;
            }
        }

        // No contradictions were found.
        return true;
    }

    /// <summary>
    /// Returns how many effects of action1 match a precondition of action2.
    /// </summary>
    /// <param name="action1">
    /// The action whose effects should be checked.
    /// </param>
    /// <param name="action2">
    /// The action whose preconditions should be checked.
    /// </param>
    /// <returns>
    /// The number of effects in action1 matching a precondition in action2.
    /// </returns>
    private int EffectsMatchingPreconditions(
        GoapAction action1,
        GoapAction action2)
    {
        int matchingConditions = 0;
        foreach (var effect in action1.Effects)
        {
            foreach (var precondition in action2.Preconditions)
            {
                if (effect.Key == precondition.Key &&
                    effect.Value.Equals(precondition.Value))
                {
                    matchingConditions++;
                }
            }
        }
        return matchingConditions;
    }
}
