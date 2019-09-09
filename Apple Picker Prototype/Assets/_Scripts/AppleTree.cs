using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleTree : MonoBehaviour {

    [Header("Set in Inspector")]
    public GameObject applePrefab;
    public GameObject wormPrefab;
    public GameObject sprayPrefab;
    public int[] probabilitiesAppleWormSpray;
    public float speed = 1f;
    public float leftAndRightEdge = 10f;
    public float chanceToChangeDirections = 0.1f;
    public float secondsBetweenAppleDrops = 1f;
    public float sprayDuration = 5;

    private List<GameObject> eventProbabilityList;

    GameObject[] drops;
    ParticleSystem ps;

    bool sprayWorking = false;
    float sprayTimePassed = 0;

    // Use this for initialization
    void Start () {
        Invoke("DropAnything", 2f);
        drops = new GameObject[3];
        drops[0] = applePrefab;
        drops[1] = wormPrefab;
        drops[2] = sprayPrefab;

        //create list 
        eventProbabilityList = new List<GameObject>();
        for (int i = 0; i < drops.Length; i++)
        {
            for(int j=0; j < probabilitiesAppleWormSpray[i]; j++)
            {
                eventProbabilityList.Add(drops[i]);
            }
        }

        ps = GetComponent<ParticleSystem>();
        var emission = ps.emission;
        emission.enabled = false;
        sprayWorking = false;
    }

    void DropAnything()
    {
        GameObject drop; 

        if(Basket.HardScore && !sprayWorking)
        {
            drop = eventProbabilityList[Random.Range(0, eventProbabilityList.Count)];
        }
        else
        {
            drop = drops[0];
        }
        GameObject dropped = Instantiate<GameObject>(drop);
        dropped.transform.position = transform.position;
        Invoke("DropAnything", secondsBetweenAppleDrops);
    }

    // Update is called once per frame
    void Update () {

        // speed up drops
        secondsBetweenAppleDrops = ((secondsBetweenAppleDrops > 0.6f) ? secondsBetweenAppleDrops -= Time.deltaTime / 100 : 0.6f);
        
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        transform.position = pos;

        if (pos.x < - leftAndRightEdge)
        {
            speed = Mathf.Abs(speed);// Move right
            speed += Time.deltaTime*3 ; // speed up appleTree movements
        }
        else if (pos.x > leftAndRightEdge)
        {
            speed = -Mathf.Abs(speed); // Move left
            speed -= Time.deltaTime*3; // speed up appleTree movements
        }

        // if spray is catched, remove worms and sprays from drops, drop only apples for sprayDuration
        if(Basket.SprayCatched && !sprayWorking)
        {
            Basket.SprayCatched = false; // return bool value to false in Basket script

            sprayWorking = true; // spray process is started

            var emission = ps.emission; // make spray emission
            emission.enabled = true;
            ps.Play();
        }

        // wait for 5 sec
        if (sprayWorking) 
        {
            sprayTimePassed += Time.deltaTime;
        }

        // when sprayDuration is expired, return worms and sprays to drops
        if (sprayTimePassed >= sprayDuration)
        {
            Basket.HardScore = true;
            sprayWorking = false;
            sprayTimePassed = 0;
        }
    }

    private void FixedUpdate()
    {
        if (Random.value < chanceToChangeDirections)
        {
            speed *= -1; // Change direction
        }
    }
}
