using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    public GameObject missleTarget;

    [SerializeField]
    private WeaponType _type;

    private float birthTime;
    private int waveFrequency = 20;
    // sine wave width in meters
    private float waveWidth = 4;
    private Vector3 pos;


    // This public property masks the field _type and takes action when it is set
    public WeaponType type
    { // c
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
            // c
        }
    }

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>(); 
        rigid = GetComponent<Rigidbody>();
        birthTime = Time.time;
        
        
    }

    private void Start()
    {
        pos = gameObject.transform.position;
    }

    void Update()
    {
        if (bndCheck.offUp)
        { // a
            Destroy(gameObject);
        }
        if(_type == WeaponType.phaser)
        {
            movePhaser();
        }
        if(_type == WeaponType.missile || _type == WeaponType.swivelGun)
        {
            moveMissile();
        }
        

    }

    /// <summary>
    /// Sets the _type private field and colors this projectile to match the
    /// WeaponDefinition.
    /// </summary>
    /// <param name="eType">The WeaponType to use.</param>
    public void SetType(WeaponType eType)
    { // e
      // Set the _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }

    void movePhaser()
    {
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        
        pos += transform.up * Time.deltaTime * def.velocity;
        gameObject.transform.position = pos + transform.right * Mathf.Sin(Time.time * waveFrequency) * 0.5f; //* waveWidth;
    }

    void moveMissile()
    {
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
       

        if (missleTarget != null)
        {
            transform.LookAt(missleTarget.transform);
            transform.position += transform.forward * def.velocity * Time.deltaTime;
        }
        else
            Destroy(gameObject);
    }


   



}
