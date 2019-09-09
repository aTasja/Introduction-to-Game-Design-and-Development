using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hero : MonoBehaviour
{
    static public Hero S; //Singleton // a

    [Header("Set in Inspector")]
    // These fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    public int Health = 100;
    public Sprite shield;
    public Sprite shieldUnchecked;
    public GameObject shieldGO;




    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1; // Remember the underscore

    // This variable holds a reference to the last triggering GameObject
    private GameObject lastTriggerGo = null;

    

    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate(); // a

    // Create a WeaponFireDelegate field named fireDelegate.
    public WeaponFireDelegate fireDelegate;

    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireMissleDelegate(); // a



    void Start()
    {
        if (S == null)
        {
            S = this; // Set the Singleton // a
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        //fireDelegate += TempFire;

        // Reset the weapons to start _Hero with one weapon
        ClearWeapons();
        
        shieldGO.GetComponent<SpriteRenderer>().sprite = shieldUnchecked;
    }

    void Update()
    {
        // Pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal"); // b
        float yAxis = Input.GetAxis("Vertical"); // b

        // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic // c
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        // Allow the ship to fire
        //if (Input.GetKeyDown(KeyCode.Space))
        //{ // a
        //    TempFire();
        //}

        // Use the fireDelegate to fire Weapons
        // First, make sure the button is pressed: Axis("Jump")
        // Then ensure that fireDelegate isn't null to avoid an error
        if (fireDelegate != null)  //Input.GetAxis("Jump") == 1 && 
        { // d
            fireDelegate(); // e
        }

        if(shieldLevel == 4)
        {
            shieldGO.GetComponent<SpriteRenderer>().sprite = shield;
            int weap = 0;
            foreach(Weapon w in weapons)
            {
                if (w.isActiveAndEnabled)
                    weap++;
            }
            if(weap == 5)
            {
                AchievementManager.CheckAchievement(weapons[0].type);
    

                Main.S.DelayedRestart(gameRestartDelay, true);
            }
        }

        if(weapons[0].type == WeaponType.laser && !gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
        else if(weapons[0].type != WeaponType.laser)
        {
            gameObject.GetComponent<AudioSource>().Stop();
        }

        if(Main.GAME_STATE == Main.GameState.level && weapons[0].type == WeaponType.none)
        {
            setWeapon();
        }

  
    }

    void setWeapon()
    {
        WeaponType currentType = WeaponType.none;
        foreach (Achievement ach in AchievementManager.GetAchievements())
        {
            if (!ach.achieved)
                currentType = ach.type;
        }

        if (currentType != WeaponType.none)
            weapons[0].SetType(currentType);
        else
        {
            Debug.Log("!!!GAME OVER!!!");
        }
    }

    void TempFire()
    { // b
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        //rigidB.velocity = Vector3.up * projectileSpeed;

        Projectile proj = projGO.GetComponent<Projectile>(); // h
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        //print("Triggered: " + other.gameObject.name);
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);

        if(go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;
        if (go.tag == "Enemy" || go.name == "_ProjectileAnchor" || go.tag == "ProjectileEnemy")
        {
            // If the shield was triggered by an enemy
            shieldLevel--; // Decrease the level of the shield by 1
            shieldGO.GetComponent<SpriteRenderer>().sprite = shieldUnchecked;
            Destroy(go); // … and Destroy the enemy // e
            AudioManager.Play(AudioClipName.Ouch);
        }
        else if (go.tag == "PowerUp")
        {
            // If the shield was triggered by a PowerUp
            AbsorbPowerUp(go);
        }

        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {

            case WeaponType.shield:
                shieldLevel++;
                AudioManager.Play(AudioClipName.Health);
                break;
            default:
                if(pu.type == weapons[0].type) // If it is the same type
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        // Set it to pu.type
                        w.SetType(pu.type);
                    }
                }
                else
                { // If this is a different weapon type // d
                    ClearWeapons();
                    weapons[0].SetType(pu.type);

                    
                }
                break;
        
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get { return (_shieldLevel); }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                ClearWeapons();
                Destroy(this.gameObject);
                // Tell Main.S to restart the game after a delay
                
                Main.S.DelayedRestart(gameRestartDelay, false);
            }
        }

    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }

    static public WeaponType GetZeroWeapon()
    {
        return S.weapons[0].type;
    }

    
}