using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy_3 : Enemy
{ // Enemy_3 extends Enemy

    // Enemy_3 will move following a Bezier curve, which is a linear
    // interpolation between more than two points.

    [Header("Set in Inspector: Enemy_3")]
    public float lifeTime = 5;
    public GameObject[] weapons;

    [Header("Set Dynamically: Enemy_3")]
    public Vector3[] points;
    public float birthTime;

    // Again, Start works well because it is not used by the Enemy superclass
    void Start()
    {
        points = new Vector3[3]; // Initialize points
                                 // The start position has already been set by Main.SpawnEnemy()
        points[0] = pos;

        // Set xMin and xMax the same way that Main.SpawnEnemy() does
        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;
        
        // Pick a random middle position in the bottom half of the screen
        Vector3 v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndCheck.camHeight * Random.Range(2.75f, 2);
        points[1] = v;

        // Pick a random final position above the top of the screen
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;

        // Set the birthTime to the current time
        birthTime = Time.time;

        Fire();

    }

    public override void Move()
    {
        // Bezier curves work based on a u value between 0 & 1
        float u = (Time.time - birthTime) / lifeTime;
        if (u > 1)
        {
            // This Enemy_3 has finished its life
            Destroy(this.gameObject);
            return;
        }
        // Interpolate the three Bezier curve points
        Vector3 p01, p12;
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);
        p01 = (1 - u) * points[0] + u * points[1];
        //p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        pos = (1 - u) * p01 + u * p12;
    }

    void Fire()
    {
        foreach(GameObject weapon in weapons)
        {
            Projectile p = weapon.GetComponent<Weapon>().MakeProjectile();
            
            p.rigid.velocity = Vector3.down *40;
        }

        Invoke("Fire", fireRate);
    }
}