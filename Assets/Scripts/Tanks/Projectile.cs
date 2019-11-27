using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Damage of Projectile
    public int Damage = 0;

    // Speed of projectile
    public float Speed = 0f;

    public Vector3 Direction;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Direction * Speed * 0.1f * Time.deltaTime);
    }
}
