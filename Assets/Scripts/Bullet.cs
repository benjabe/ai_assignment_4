using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bullet;
    public int speed;
    public int damage;
    void Start()
    {
        speed = 10;
        damage = 50;
    }

    void Update()
    {
        move();
    }

    void move()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}
