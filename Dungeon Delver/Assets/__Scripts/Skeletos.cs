using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeletos : Enemy,  IFacingMover
{ // a
    [Header("Set in Inspector: Skeletos")] // b
    public int speed = 2;
    public float timeThinkMin = 1f;
    public float timeThinkMax = 4f;

    [Header("Set Dynamically: Skeletos")]
    public int facing = 0;
    public float timeNextDecision = 0;

    private InRoom inRm;

    protected override void Awake()
    {
        base.Awake();
        inRm = GetComponent<InRoom>();
    }

    override protected void Update()
    {
        base.Update();
        if (knockback) return;

        if (Time.time >= timeNextDecision)
        { // c
            DecideDirection();
        }
        // rigid is inherited from Enemy and is initialized in Enemy.Awake()
        rigid.velocity = directions[facing] * speed;
    }

    void DecideDirection()
    { // d
        facing = Random.Range(0, 4);
        timeNextDecision = Time.time +
        Random.Range(timeThinkMin, timeThinkMax);
    }

    // Implementation of IFacingMover
    public int GetFacing()
    {
        return facing;
    }
    public bool moving { get { return true; } }
    public float GetSpeed()
    {
        return speed;
    }
    public float gridMult
    {
        get { return inRm.gridMult; }
    }
    public Vector2 roomPos
    {
        get { return inRm.roomPos; }
        set { inRm.roomPos = value; }
    }
    public Vector2 roomNum
    {
        get { return inRm.roomNum; }
        set { inRm.roomNum = value; }
    }
    public Vector2 GetRoomPosOnGrid(float mult = -1)
    {
        return inRm.GetRoomPosOnGrid(mult);
    }
}