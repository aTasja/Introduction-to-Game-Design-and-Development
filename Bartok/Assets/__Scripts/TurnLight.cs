using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLight : MonoBehaviour {

    void Update()
    {
        transform.position = Vector3.back * 3;
        // a
        if (Bartok.CURRENT_PLAYER == null)
        {
            // b
            return;
        }
        transform.position += Bartok.CURRENT_PLAYER.handSlotDef.pos;
        // c
    }
}
