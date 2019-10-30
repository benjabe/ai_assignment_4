using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An Action available in the goal oriented action planning system
/// </summary>
public class GoapAction : MonoBehaviour
{
    /// <summary>
    /// The preconditions for the action to be performed.
    /// </summary>
    public List<string> Preconditions = null;

    /// <summary>
    /// The change in world state as a result of the action being performed.
    /// </summary>
    public List<string> Postconditions = null;

    /// <summary>
    /// Execute the action.
    /// </summary>
    /// <param name="actor">The actor who executes the action.</param>
    public void Execute(GoapActor actor) {}
}
