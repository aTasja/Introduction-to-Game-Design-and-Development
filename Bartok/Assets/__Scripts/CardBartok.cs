using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CBState includes both states for the game and to… states for movement
// a
public enum CBState
{
    toDrawpile,
    drawpile,
    toHand,
    hand,
    toTarget,
    target,
    discard,
    to,
    idle
}

public class CardBartok : Card
{
    // b
    // Static variables are shared by all instances of CardBartok
    static public float MOVE_DURATION = 0.5f;
    static public string MOVE_EASING = Easing.InOut;
    static public float CARD_HEIGHT = 3.5f;
    static public float CARD_WIDTH = 2f;

    [Header("Set Dynamically: CardBartok")]
    public CBState state = CBState.drawpile;
    // Fields to store info the card will use to move and rotate
    public List<Vector3> bezierPts;
    public List<Quaternion> bezierRots;
    public float timeStart, timeDuration;
    public int eventualSortOrder;
    public string eventualSortLayer;

    // When the card is done moving, it will call reportFinishTo.SendMessage()
    public GameObject reportFinishTo = null;

    [System.NonSerialized]    // a
    public Player callbackPlayer = null; // b

    // MoveTo tells the card to interpolate to a new position and rotation
    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        // Make new interpolation lists for the card.
        // Position and Rotation will each have only two points.
        bezierPts = new List<Vector3>();
        bezierPts.Add(transform.localPosition); // Current position
        bezierPts.Add(ePos); // Current rotation
        bezierRots = new List<Quaternion>();
        bezierRots.Add(transform.rotation); // New position
        bezierRots.Add(eRot); // New rotation
        if (timeStart == 0)
        { // c
            timeStart = Time.time;
        }

        // timeDuration always starts the same but can be overwritten later
        timeDuration = MOVE_DURATION;
        state = CBState.to;
        // d
    }

    public void MoveTo(Vector3 ePos)
    {
        // e
        MoveTo(ePos, Quaternion.identity);
    }

    void Update()
    {
        switch (state)
        { // f
            case CBState.toHand:
            case CBState.toTarget:
            case CBState.toDrawpile:
            case CBState.to:
                float u = (Time.time - timeStart) / timeDuration;
                // g
                float uC = Easing.Ease(u, MOVE_EASING);

                if (u < 0)
                { // h
                    transform.localPosition = bezierPts[0];
                    transform.rotation = bezierRots[0];
                    return;
                }

                else if (u >= 1)
                { // i
                    uC = 1;
                    // Move from the to... state to the proper next state
                    if (state == CBState.toHand) state = CBState.hand;
                    if (state == CBState.toTarget) state = CBState.target;
                    if (state == CBState.toDrawpile) state = CBState.drawpile;
                    if (state == CBState.to) state = CBState.idle;

                    // Move to the final position
                    transform.localPosition = bezierPts[bezierPts.Count - 1];
                    transform.rotation = bezierRots[bezierPts.Count - 1];

                    // Reset timeStart to 0 so it gets overwritten next time
                    timeStart = 0;
                    if (reportFinishTo != null)
                    { // j
                        reportFinishTo.SendMessage("CBCallback", this);
                        reportFinishTo = null;
                    }
                    else if(callbackPlayer != null)
                    {
                        // If there's a callback Player
                        // Call CBCallback directly on the Player
                        callbackPlayer.CBCallback(this);
                        callbackPlayer = null;
                    }
                    else
                    { // If there is nothing to callback
                      // Just let it stay still.
                    }
                }

                else
                { // Normal interpolation behavior (0 <= u < 1) // k
                    Vector3 pos = Utils.Bezier(uC, bezierPts);
                    transform.localPosition = pos;
                    Quaternion rotQ = Utils.Bezier(uC, bezierRots);
                    transform.rotation = rotQ;

                    if (u > 0.5f)
                    {
                        // a
                        SpriteRenderer sRend = spriteRenderers[0];
                        if (sRend.sortingOrder != eventualSortOrder)
                        {
                            // Jump to the proper sort order
                            SetSortOrder(eventualSortOrder);
                        }
                        if (sRend.sortingLayerName != eventualSortLayer)
                        {
                            // Jump to the proper sort layer
                            SetSortingLayerName(eventualSortLayer);
                        }
                    }
                }
                break;
        }
    }

    // This allows the card to react to being clicked
    override public void OnMouseUpAsButton()
    {
        // Call the CardClicked method on the Bartok singleton
        Bartok.S.CardClicked(this);
        // Also call the base class (Card.cs) version of this method
        base.OnMouseUpAsButton();
    }
}