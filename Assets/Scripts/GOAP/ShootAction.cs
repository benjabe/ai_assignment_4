using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GoapAction. Shoots a shell at a target.
/// </summary>
public class ShootAction : GoapAction
{
    /// <summary>
    /// Prefab of shell to instantiate when shooting.
    /// </summary>
    [SerializeField] private GameObject _shellPrefab = null;

    /// <summary>
    /// Where to instantiate shells.
    /// </summary>
    [SerializeField] private Transform _shellInstantiationPoint = null;

    /// <summary>
    /// The time it takes to cooldown after shooting.
    /// The actor cannot do anything during this time.
    /// </summary>
    //[SerializeField] private float _coolDownTime = 1.0f;

    /// <summary>
    /// True if the action has been succesfully performed.
    /// </summary>
    private bool _isDone = false;

    public ShootAction()
    {
        // In order to shoot we need to have ammo, and a target
        AddPrecondition("hasAmmo", true);
        AddPrecondition("hasTarget", true);

        // We spend ammo when shooting and it might kill the enemy.
        AddEffect("hasAmmo", false);
        AddEffect("targetIsDead", true);
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
        // This assumes that the barrel is aimed towards the target
        Tank tank = agent.GetComponent<Tank>();
        if ((bool)tank.Conditions["hasAmmo"] == true)
        {
            // Fire a projectile! Hopefully it will hit the enemy.
            // Regardless, we have to spend our ammo.
            tank.Conditions["hasAmmo"] = false;

            // Instantiate the shell prefab at the instantiation point
            // then remove the parent to avoid weird stuff if the parent
            // gets destroyed
            GameObject shellObject = Instantiate(
                _shellPrefab,
                _shellInstantiationPoint,
                false
            );
            shellObject.transform.parent = null;
            _isDone = true;
            return true;
        }
        else
        {
            // Agent doesn't have ammo, so can't shoot.
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
        return false;
    }
}
