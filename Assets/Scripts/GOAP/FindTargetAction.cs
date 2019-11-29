using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTargetAction : GoapAction
{
    /// <summary>
    /// True if the action is done performing.
    /// </summary>
    private bool _isDone = false;

    public FindTargetAction()
    {
        // It doesn't make senes to find a target if we already have one.
        AddPrecondition("hasTarget", false);

        // The effect is obvious.
        AddEffect("hasTarget", true);
    }

    /**
     * Reset any variables that need to be reset before planning happens again.
     */
    public override void Reset()
    {
        _isDone = false;
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

        // Try to get a target in sight.
        Tank target = tank.FindTankInLineOfSight();
        if (target != null)
        {
            // Target in sight
            tank.Target = target;
            _isDone = true;
        }
        else
        {
            // No target in sight
            // Move somewhere else.
            if (tank.Path == null)
            {
                // No path, create a new one
                tank.Path = tank.Level.ConstructPath(
                    tank.CurrentTile,
                    tank.Level.Tiles[Random.Range(0, tank.Level.Tiles.Count)]
                );
            }
            if (tank.Path != null)
            {
                // Tank has a path, move along it
                tank.MoveAlongPath();
            }
        }
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
