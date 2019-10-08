using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour {

    private GameObject sword;
    private Dray dray;

    void Start()
    {
        sword = transform.Find("Sword").gameObject; // a
        dray = transform.parent.GetComponent<Dray>();
        
        // Deactivate the sword
        sword.SetActive(false); // b
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90 * dray.facing); // c
        sword.SetActive(dray.mode == Dray.eMode.attack); // d
    }
}