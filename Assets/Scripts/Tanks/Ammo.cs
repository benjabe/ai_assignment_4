using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents data about the ammo used by a Turret
public class Ammo
{
    // Time in seconds between each shot
    public float fireRate = 1.25f;

    // Damage per shot
    public int damage = 25;

    public Ammo(float inFireRate, int inDamage)
    {
        fireRate = inFireRate;
        damage = inDamage;
    }
}
