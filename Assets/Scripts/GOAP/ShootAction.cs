using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : GoapAction {
    private bool inRange = false;

    private float startTime = 0;



    // Start is called before the first frame update                           
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ShootAction() {
        AddPrecondition("hasAmmo", true);
        AddPrecondition("inRange", true);
        AddPrecondition("mustReload", false);
        AddEffect("mustReload", true);
    }

    // We need to be in range (have vision) of the enemy tank in order to shoot
    public override bool RequiresInRange() {
        return true;
    }


    public override void Reset() {
        inRange = false;
        startTime = 0;
    }

    public override bool IsDone() {
        throw new System.NotImplementedException();
    }

    public override bool CheckProceduralPrecondition(GameObject agent) {
        throw new System.NotImplementedException();
    }

    public override bool Perform(GameObject agent) {
        throw new System.NotImplementedException();
    }
}
