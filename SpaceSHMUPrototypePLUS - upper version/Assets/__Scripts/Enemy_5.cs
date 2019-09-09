using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_5 : Enemy {

    [Header("Set Dynamically: Enemy_5")]


    [Tooltip("Optional offset from the target's position to move towards.")]
    public Vector3 followOffset;

    [Tooltip("Speed to move towards the target.")]
    public float followSpeed;

    [Tooltip("True if this object should rotate to face the target.")]
    public bool lookAtTarget;

    [Tooltip("Speed to rotate towards the target.")]
    public float lookSpeed;

    public float birthTime;

    private Transform target;

    // Start works well because it's not used by the Enemy superclass
    void Start()
    {
        try
        {
            target = GameObject.FindGameObjectWithTag("Hero").transform;
                lookAtTarget = true;
            birthTime = Time.time;
        }
        catch (Exception e)
        {
            Destroy(gameObject);
            print(e);
        }

        
    }


    private void LateUpdate()
    {
        
        if (target != null)
        {
            // move towards the target position ( plus the offset ), never moving farther than "followSpeed" in one frame.
            transform.position = Vector3.MoveTowards(transform.position, target.position + followOffset, followSpeed);

            // get a rotation that points Z axis forward, and the Y axis towards the target
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, (target.position - transform.position));

            // rotate toward the target rotation, never rotating farther than "lookSpeed" in one frame.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, lookSpeed);

            // rotate 90 degrees around the Z axis to point X axis instead of Y
            //transform.Rotate(180, 0, 0);
        }
        else
        {
            Destroy(gameObject);
        }
       


    }

}
