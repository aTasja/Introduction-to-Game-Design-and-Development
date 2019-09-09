using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is an enum of the various possible weapon types.
/// It also includes a "shield" type to allow a shield power-up.
/// Items marked [NI] below are Not Implemented in the IGDPD book.
/// </summary>
public enum WeaponType
{
    none, // The default / no weapon
    blaster, // A simple blaster
    spread, // Two shots simultaneously
    phaser, // [NI] Shots that move in waves
    missile, // [NI] Homing missiles
    laser, // [NI]Damage over time
    swivelGun, // [NI] Shots toward the nearest enemy
    shield // Raise shieldLevel
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties
/// of a specific weapon in the Inspector. The Main class has
/// an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]               // a
public class WeaponDefinition
{ // b
    public WeaponType type = WeaponType.none;
    public string letter;               // Letter to show on the power-up
    public Color color = Color.white;   // Color of Collar & power-up
    public GameObject projectilePrefab; // Prefab for projectiles
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;       // Amount of damage caused
    public float continuousDamage = 0;  // Damage per second (Laser)
    public float delayBetweenShots = 0;
    public float velocity = 20;         // Speed of  projectiles
    public bool achieved;
    public GameObject ahivementSign;

}

public class Weapon : MonoBehaviour {

    static public Transform PROJECTILE_ANCHOR;

    [Header("Set in the Inspector")]
    public GameObject lightCone;


    [Header("Set Dynamically")] [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // Time last shot was fired
    


    private Renderer collarRend;
    //laser
    private LineRenderer laser;
    private int _laserLength = 100;
    private float continuousTimeOfDamage;
    private string laseredEnemy = null;
    //missle
    
    private bool readyToFireMissle;
    private GameObject missleTarget;



    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();
        laser = collar.GetComponent<LineRenderer>();

        readyToFireMissle = false;




        // Call SetType() for the default _type of WeaponType.none
        SetType(_type); // a
        
        // Dynamically create an anchor for all Projectiles
        if (PROJECTILE_ANCHOR == null)
        { // b
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // Find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject; // c
        if (rootGO.GetComponent<Hero>() != null)
        { // d
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }
    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }
    public void SetType(WeaponType wt)
    {
        ResetWeaponFeatures();
        _type = wt;
        if (type == WeaponType.none)
        { // e
            this.gameObject.SetActive(false);
            
            return;
        }
        else if(type == WeaponType.laser)
        {
            this.gameObject.SetActive(true);
            laser.enabled = true;

        }
        else if (type == WeaponType.missile)
        {
            this.gameObject.SetActive(true);
            lightCone.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type); // f
        collarRend.material.color = def.color;
        lastShotTime = 0; // You can fire immediately after _type is set. // g
}
    public void Fire()
    {
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;   // h
        // If it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        { // i
            return;
        }
        Projectile p;
        
        Vector3 vel = Vector3.up * def.velocity; // j
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        { // k
            case WeaponType.blaster:

                p = MakeProjectile();
                p.rigid.velocity = vel;
                AudioManager.Play(AudioClipName.Blaster);
                break;

            case WeaponType.spread: // l

                p = MakeProjectile(); // Make middle Projectile
                p.rigid.velocity = vel;

                p = MakeProjectile(); // Make right Projectile
                p.transform.rotation = Quaternion.AngleAxis(5, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile(); // Make right Projectile
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile(); // Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-5, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile(); // Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                AudioManager.Play(AudioClipName.Spread);

                break;

            case WeaponType.phaser:

                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(5, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel; ;

                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-5, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel; ;

                AudioManager.Play(AudioClipName.Phaser);

                break;

            case WeaponType.laser:

                
                break;

            case WeaponType.missile:
                readyToFireMissle = true;
                

                break;

            case WeaponType.swivelGun:
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                float dist = 999999;
                foreach(GameObject go in enemies)
                {
                    if (Vector3.Distance(go.transform.position, transform.position) < dist)
                    {
                        dist = Vector3.Distance(go.transform.position, transform.position);
                        missleTarget = go;
                        

                        AudioManager.Play(AudioClipName.Gun);
                    }
                }

                p = MakeProjectile();
                p.missleTarget = missleTarget;



                break;

        }
        Invoke("Fire", Main.GetWeaponDefinition(type).delayBetweenShots);
    }
    
    public Projectile MakeProjectile()
    { // m
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        { // n
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true); // o
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time; // p
        return (p);
    }

    void hitLaser()
    {
        if (!laser.enabled)
            laser.enabled = true;
        Vector3 endPosition = transform.position + (transform.up * _laserLength);
        laser.SetPositions(new Vector3[] { transform.position, endPosition });
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit))
        {
            if (hit.collider)
            {
                endPosition = transform.position + (transform.up * hit.distance);
                laser.SetPositions(new Vector3[] { transform.position, endPosition });
                //print("HIT! " + hit.collider.gameObject.transform.root.gameObject.name);

                if(hit.collider.gameObject.transform.parent != null &&
                    hit.collider.gameObject.transform.parent.gameObject.tag == "Enemy")
                {
                    if (laseredEnemy == hit.collider.gameObject.transform.parent.name)
                    {
                        GameObject parent = hit.collider.gameObject.transform.parent.gameObject;
                        float h = parent.GetComponent<Enemy>().health;
                        float d = Main.GetWeaponDefinition(WeaponType.laser).continuousDamage;
                        float deltaTime = Time.time - continuousTimeOfDamage;
                        parent.GetComponent<Enemy>().health -= d * deltaTime;
                        if (h <= 0)
                        {
                            Main.S.ShipDestroyed(parent.GetComponent<Enemy>());
                            Destroy(parent);
                        }
                    }
                    else
                    {
                        laseredEnemy = hit.collider.gameObject.transform.parent.name;
                        continuousTimeOfDamage = Time.time;
                    }
                }

            }
        }
        else
        {
            laser.SetPosition(1, endPosition);
        }
        
    }

    private void Update()
    {
        if (laser.enabled)
        {
            hitLaser();
        }

        if (readyToFireMissle && SearchForMissleTarget() != null)
        {
 
            readyToFireMissle = false;
            Projectile missileProjectile = MakeProjectile();
            AudioManager.Play(AudioClipName.Missile);
            missileProjectile.missleTarget = SearchForMissleTarget();

        }
        if (Time.time - lastShotTime > def.delayBetweenShots && type == WeaponType.missile)
        {
            readyToFireMissle = true;
        }
    }

    public GameObject SearchForMissleTarget()
    {
        RaycastHit[] hits = lightCone.GetComponent<LightCone>().GetRaycastHits();
        GameObject target = null;
        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider == null)
                {
                    continue;
                }
                if (hits[i].collider.transform.parent.gameObject.tag == "Enemy")
                {
                    target = hits[i].collider.transform.parent.gameObject;
                    break;
                }
            }
        }
        return target;
    }

    void ResetWeaponFeatures()
    {
        lightCone.SetActive(false);
        laser.enabled = false;
    }

   
}