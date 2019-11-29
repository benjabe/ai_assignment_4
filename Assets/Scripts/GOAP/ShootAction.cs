using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : GoapAction 
{
    private bool shot;
    private Tank enemy;
    private float startTime;



    // Start is called before the first frame update                           
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ShootAction() 
    {
        AddPrecondition("hasAmmo", true);
        AddPrecondition("inRange", true);
        AddPrecondition("mustReload", false);
        AddEffect("mustReload", true);
    }

    // We need to be in range (have vision) of the enemy tank in order to shoot
    public override bool RequiresInRange() 
    {
        return true;
    }


    public override void Reset() 
    {
        shot = false;
        startTime = 0;
        enemy = null;
    }

    public override bool IsDone() 
    {
        return shot;
    }

    /// <summary>
    /// Checks if we actually have vision of at least one enemy tank
    /// </summary>
    /// <param name="agent">
    /// The agent trying to perform the ShootAction on another tank
    /// </param>
    /// <returns>
    /// True if there is one or more enemy tanks in vision-range
    /// False if no enemy tanks are in vision
    /// </returns>
    public override bool CheckProceduralPrecondition(GameObject agent) 
    {
        Tank ourTank = (Tank)agent.GetComponent(typeof(Tank));
        Tank[] tanks = FindObjectsOfType(typeof(Tank)) as Tank[];
        Tank closestEnemy = null;
        float closestDistance = 0;

        // Checks every tank
        foreach (Tank tank in tanks)
        {
            // Checks that the tank is not ourselves, and that it is in vision
            if (tank != ourTank && tank == ourTank.FindTankInLineOfSight()) 
            {
                // Check if it is closer than the previous closest one
                float newDistance = (tank.gameObject.transform.position - agent.transform.position).magnitude;

                // If this is the first tank we have found in vision
                if (closestDistance == 0) {
                    // Then set it as the closest enemy
                    closestEnemy = tank;
                    closestDistance = newDistance;
                } 
                    
                // Not the first tank we have found, but is it closer than the previous one?
                else 
                {
                    if (newDistance < closestDistance) 
                    {
                        // The new tank we found is closer than the previous one, set it as the closest enemy
                        closestEnemy = tank;
                        closestDistance = newDistance;
                    }
                }
            } 
        }

        if (closestEnemy != null) {
            enemy = closestEnemy;
            target = enemy.gameObject;
            return true;
        }

        return false;
    }

    public override bool Perform(GameObject agent) 
    {
        Tank tank = (Tank)agent.GetComponent(typeof(Tank));
        tank.Shoot();

        return true;
    }
}
