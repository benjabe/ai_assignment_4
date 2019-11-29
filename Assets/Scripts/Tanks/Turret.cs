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

    // Speed of projectile
    [SerializeField] private float _projectileSpeed = 0.01f;

    // Time until we can shoot again
    private float _timeUntilCanShoot = 0f;

    // Game object of turret
    public GameObject TurretMesh;

    // Spawn point of projectile
    public GameObject ProjectileSpawnPoint;

    // The projectile game object that should be spawned
    public GameObject ProjectileObject;

    void Update()
    {
        _timeUntilCanShoot -= Time.deltaTime;
    }

    public bool Shoot(GameObject target)
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
            var bullet = Instantiate(ProjectileObject, ProjectileSpawnPoint.transform.position, transform.rotation);
            var projectile = bullet.GetComponent<Projectile>();

            // Set projectile properties based on Turret Stats
            projectile.Damage = _damage;
            projectile.Speed = _projectileSpeed;
            projectile.Direction = (target.transform.position - transform.position).normalized;

        }
        // If we are empty, but not reloading, start reload
        else if(!_magazine.IsReloading())
        {        
            _magazine.StartCoroutine("Reload");
        }

        return couldShoot;
    }
}
