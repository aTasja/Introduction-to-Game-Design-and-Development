using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMove : MonoBehaviour {

    private IFacingMover mover;

    void Awake()
    {
        mover = GetComponent<IFacingMover>
        (); // a
    }
    void FixedUpdate()
    {
        if (!mover.moving) return; // If not moving, nothing to do here
        int facing = mover.GetFacing();
        // If we are moving in a direction, align to the grid
        // First, get the grid location
        Vector2 rPos = mover.roomPos;
        Vector2 rPosGrid = mover.GetRoomPosOnGrid();
        // This relies on IFacingMover (which uses InRoom) to choose grid spacing
        // Then move towards the grid line
        float delta = 0;
        if (facing == 0 || facing == 2)
        {
            // Horizontal movement, align to y grid
            delta = rPosGrid.y - rPos.y;
        }
        else
        {
            // Vertical movement, align to x grid
            delta = rPosGrid.x - rPos.x;
        }
        if (delta == 0) return; // Already aligned to the grid
        float move = mover.GetSpeed() * Time.fixedDeltaTime;
        move = Mathf.Min(move, Mathf.Abs(delta));
        if (delta < 0) move = -move;
        if (facing == 0 || facing == 2)
        {
            // Horizontal movement, align to y grid
            rPos.y += move;
        }
        else
        {
            // Vertical movement, align to x grid
            rPos.x += move;
        }
        mover.roomPos = rPos;
    }
}
