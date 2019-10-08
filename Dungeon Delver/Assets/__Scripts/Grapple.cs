using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {

    public enum eMode { none, gOut, gInMiss, gInHit }

    [Header("Set in Inspector")]
    public float grappleSpd = 10;
    public float grappleLength = 7;
    public float grappleInLength = 0.5f;
    public int unsafeTileHealthPenalty = 2;
    public TextAsset mapGrappleable;

    [Header("Set Dynamically")]
    public eMode mode = eMode.none;
    // TileNums that can be grappled
    public List<int> grappleTiles;
    public List<int> unsafeTiles;

    private Dray dray;
    private Rigidbody rigid;
    private Animator anim;
    private Collider drayColld;
    private GameObject grapHead;
    private LineRenderer grapLine;
    private Vector3 p0, p1;
    private int facing;

    private Vector3[] directions = new Vector3[] {
        Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    void Awake()
    {
        string gTiles = mapGrappleable.text;
        gTiles = Utils.RemoveLineEndings(gTiles);
        grappleTiles = new List<int>();
        unsafeTiles = new List<int>();
        for (int i = 0; i < gTiles.Length; i++)
        {
            switch (gTiles[i])
            {
                case 'S':
                    grappleTiles.Add(i);
                    break;
                case 'X':
                    unsafeTiles.Add(i);
                    break;
            }
        }
        dray = GetComponent<Dray>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        drayColld = GetComponent<Collider>();
        Transform trans = transform.Find("Grappler");
        grapHead = trans.gameObject;
        grapLine = grapHead.GetComponent<LineRenderer>();
        grapHead.SetActive(false);
    }

    void Update()
    {
        if (!dray.hasGrappler) return;
        switch (mode)
        {
            case eMode.none:
                // If the grapple button is pressed
                if (Input.GetKeyDown(KeyCode.X))
                {
                    StartGrapple();
                }
                break;
        }
    }

    void StartGrapple()
    {
        facing = dray.GetFacing();
        dray.enabled = false;
        anim.CrossFade("Dray_Attack_" + facing, 0);
        drayColld.enabled = false;
        rigid.velocity = Vector3.zero;
        grapHead.SetActive(true);
        p0 = transform.position + (directions[facing] * 0.5f);
        p1 = p0;
        grapHead.transform.position = p1;
        grapHead.transform.rotation = Quaternion.Euler(0, 0, 90 * facing);
        grapLine.positionCount = 2;
        grapLine.SetPosition(0, p0);
        grapLine.SetPosition(1, p1);
        mode = eMode.gOut;
    }
    void FixedUpdate()
    {
        switch (mode)
        {
            case eMode.gOut: // Grappler shooting out // i
                p1 += directions[facing] * grappleSpd * Time.fixedDeltaTime;
                grapHead.transform.position = p1;
                grapLine.SetPosition(1, p1);
                // Check to see whether the grapple hit anything
                int tileNum = TileCamera.GET_MAP(p1.x, p1.y);
                if (grappleTiles.IndexOf(tileNum) != -1)
                {
                    // We've hit a grappleable tile!
                    mode = eMode.gInHit;
                    break;
                }
                if ((p1 - p0).magnitude >= grappleLength)
                {
                    // The grapple reached its end and didn't hit anything
                    mode = eMode.gInMiss;
                }
                break;
            case eMode.gInMiss: // Grappler missed; return at double speed // j
                p1 -= directions[facing] * 2 * grappleSpd * Time.fixedDeltaTime;
                if (Vector3.Dot((p1 - p0), directions[facing]) > 0)
                {
                    // The grapple is still in front of Dray
                    grapHead.transform.position = p1;
                    grapLine.SetPosition(1, p1);
                }
                else
                {
                    StopGrapple();
                }
                break;
            case eMode.gInHit: // Grappler hit, pulling Dray to wall // k
                float dist = grappleInLength + grappleSpd * Time.fixedDeltaTime;
                if (dist > (p1 - p0).magnitude)
                {
                    p0 = p1 - (directions[facing] * grappleInLength);
                    transform.position = p0;
                    StopGrapple();
                    break;
                }
                p0 += directions[facing] * grappleSpd * Time.fixedDeltaTime;
                transform.position = p0;
                grapLine.SetPosition(0, p0);
                grapHead.transform.position = p1;
                break;
        }
    }

    void StopGrapple()
    {
        dray.enabled = true;
        drayColld.enabled = true;
        // Check for unsafe tile
        int tileNum = TileCamera.GET_MAP(p0.x, p0.y);
        if (mode == eMode.gInHit && unsafeTiles.IndexOf(tileNum) != -1)
        {
            // We landed on an unsafe tile
            dray.ResetInRoom(unsafeTileHealthPenalty);
        }
        grapHead.SetActive(false);
        mode = eMode.none;
    }

    void OnTriggerEnter(Collider colld)
    {
        Enemy e = colld.GetComponent<Enemy>();
        if (e == null) return;
        mode = eMode.gInMiss;
    }
}