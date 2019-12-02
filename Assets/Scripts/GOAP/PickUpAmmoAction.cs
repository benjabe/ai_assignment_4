using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAmmoAction : GoapAction
{
    /// <summary>
    /// True if the action is done performing.
    /// </summary>
    private bool _isDone = false;

    /// <summary>
    /// Whether we've created a path to a ShellPickup.
    /// </summary>
    private bool _createdPath = false;

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
        _isDone = false;
        _createdPath = false;
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
        Tank tank = agent.GetComponent<Tank>();

        if ((bool)tank.Conditions["hasAmmo"] == false)
        {
            ShellPickup[] pickups = FindObjectsOfType<ShellPickup>();
            if (pickups.Length > 0)
            {
                if (!_createdPath)
                {
                    tank.Path = tank.Level.ConstructPath(
                        tank.CurrentTile,
                        pickups[0].Tile
                    );
                }
                if (tank.Path != null)
                {
                    _createdPath = true;
                    // Tank has a path, move along it
                    tank.MoveAlongPath();
                    if (tank.Path == null)
                    {
                        // Arrived at destination, pick up ammo.
                        tank.Conditions["hasAmmo"] = true;
                        _isDone = true;
                        // Moves the ammo pickup to a new destination
                        pickups[0].SetNewPosition();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            // Agent already has ammo.
            return false;
        }
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
