using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // The magazine currently used by the turret
    [SerializeField] private Magazine _magazine = new Magazine();

    // Time in seconds between each shot
    [SerializeField] private float _fireRate = 1.25f;

    // Damage per shot
    [SerializeField] private int _damage = 25;

    // Time until we can shoot again
    private float _timeUntilCanShoot = 0f;

    // Game object of turret
    public GameObject TurretMesh;

    void Update()
    {
        _timeUntilCanShoot -= Time.deltaTime;
    }

    public bool Shoot()
    {
        // Can not shoot if we are on fire rate cool down
        if (_timeUntilCanShoot > 0f)
        {
            return false;
        }

        // Next, attempt to shoot with our magazine
        var couldShoot = _magazine.Shoot();
        if(couldShoot)
        {
            _timeUntilCanShoot = _fireRate;
            // TODO: Spawn some bullet and fire it
        }
        // If we are empty, but not reloading, start reload
        else if(!_magazine.IsReloading())
        {        
            _magazine.StartCoroutine("Reload");
        }

        return couldShoot;
    }
}
