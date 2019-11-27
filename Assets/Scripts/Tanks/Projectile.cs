using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Damage of Projectile
    public int Damage = 0;

    // Speed of projectile
    public float Speed = 0f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward * Speed * Time.deltaTime);
    }
}
