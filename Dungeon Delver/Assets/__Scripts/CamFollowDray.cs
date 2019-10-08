using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowDray : MonoBehaviour {

    static public bool TRANSITIONING = false;
    [Header("Set in Inspector")]
    public InRoom drayInRm;
    public float transTime = 0.5f;
    private Vector3 p0, p1;
    private InRoom inRm;
    private float transStart;
    void Awake()
    {
        inRm = GetComponent<InRoom>();
    }
    void Update()
    {
        if (TRANSITIONING)
        {
            float u = (Time.time - transStart) / transTime;
            if (u >= 1)
            {
                u = 1;
                TRANSITIONING = false;
            }
            transform.position = (1 - u) * p0 + u * p1;
        }
        else
        {
            if (drayInRm.roomNum != inRm.roomNum)
            {
                TransitionTo(drayInRm.roomNum);
            }
        }
    }
    void TransitionTo(Vector2 rm)
    {
        p0 = transform.position;
        inRm.roomNum = rm;
        p1 = transform.position + (Vector3.back * 10);
        transform.position = p0;
        transStart = Time.time;
        TRANSITIONING = true;
    }
}