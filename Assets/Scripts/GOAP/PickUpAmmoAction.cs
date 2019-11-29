using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAmmoAction : GoapAction
{
    /// <summary>
    /// True if the action is done performing.
    /// </summary>
    private bool _isDone = false;

    public PickUpAmmoAction()
    {
        // It doesn't make senes to pick up ammo when we already have it!
        AddPrecondition("hasAmmo", false);

        // The effect is obvious.
        AddEffect("hasAmmo", true);
    }

    /**
     * Reset any variables that need to be reset before planning happens again.
     */
    public override void Reset()
    {

    }
 
    /**
     * Is the action done?
     */
    public override bool IsDone()
    {
        return _isDone;
    }
 
    /**
     * Procedurally check if this action can run. Not all actions
     * will need this, but some might.
     */
    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        return false;
    }
 
    /**
     * Run the action.
     * Returns True if the action performed successfully or false
     * if something happened and it can no longer perform. In this case
     * the action queue should clear out and the goal cannot be reached.
     */
    public override bool Perform(GameObject agent)
    {
        agent.GetComponent<Tank>().Conditions["hasAmmo"] = true;
        _isDone = true;
        return true;
    }
 
    /**
     * Does this action need to be within range of a target game object?
     * If not then the moveTo state will not need to run for this action.
     */
     // Why is this a method?
    public override bool RequiresInRange()
    {
        return true;
    }
}
