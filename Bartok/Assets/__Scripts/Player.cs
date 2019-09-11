using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Enables LINQ queries, which will be explained soon

// The player can either be human or an ai
public enum PlayerType
{
    human,
    ai
}

[System.Serializable]
public class Player
{ // b
    public PlayerType type = PlayerType.ai;
    public int playerNum;
    public SlotDef handSlotDef;
    public List<CardBartok> hand; // The cards in this player's hand
    
    // Add a card to the hand
    public CardBartok AddCard(CardBartok eCB)
    {
        if (hand == null) hand = new List<CardBartok>();
        // Add the card to the hand
        hand.Add(eCB);

        // Sort the cards by rank using LINQ if this is a human
        if (type == PlayerType.human)
        {
            CardBartok[] cards = hand.ToArray();
            // a
            // This is the LINQ call
            cards = cards.OrderBy(cd => cd.rank).ToArray();
            // b
            hand = new List<CardBartok>(cards);
            // c
            // Note: LINQ operations can be a bit slow (like it could take a
            // couple of milliseconds), but since we're only doing it once
            // every round, it isn't a problem.
        }

        eCB.SetSortingLayerName("10"); // Sorts the moving card to the top // a
        eCB.eventualSortLayer = handSlotDef.layerName;

        FanHand();
        return (eCB);
    }

    // Remove a card from the hand
    public CardBartok RemoveCard(CardBartok cb)
    {
        // If hand is null or doesn't contain cb, return null
        if (hand == null || !hand.Contains(cb)) return null;
        hand.Remove(cb);
        FanHand();
        return (cb);
    }

    public void FanHand()
    { // a
      // startRot is the rotation about Z of the first card // b
        float startRot = 0;
        startRot = handSlotDef.rot;
        if (hand.Count > 1)
        {
            startRot += Bartok.S.handFanDegrees * (hand.Count - 1) / 2;
        }

        // Move all the cards to their new positions
        Vector3 pos;
        float rot;
        Quaternion rotQ;
        for (int i = 0; i < hand.Count; i++)
        {
            rot = startRot - Bartok.S.handFanDegrees * i;
            rotQ = Quaternion.Euler(0, 0, rot); // c
            pos = Vector3.up * CardBartok.CARD_HEIGHT / 2f; // d
            pos = rotQ * pos; // e

            // Add the base position of the player's hand (which will be at the
            // bottom-center of the fan of the cards)
            pos += handSlotDef.pos; // f
            pos.z = -0.5f * i; // g

            // If not the initial deal, start moving the card immediately.
            if (Bartok.S.phase != TurnPhase.idle)
            {
                hand[i].timeStart = 0;
            }

            // Set the localPosition and rotation of the ith card in the hand
            hand[i].MoveTo(pos, rotQ); // Tell CardBartok to interpolate
            hand[i].state = CBState.toHand;
            // After the move, CardBartok will set the state to CBState.hand
            
            /*
            hand[i].transform.localPosition = pos; // h
            hand[i].transform.rotation = rotQ;
            hand[i].state = CBState.hand;
            */

            hand[i].faceUp = (type == PlayerType.human); // i
            // Set the SortOrder of the cards so that they overlap properly
            hand[i].eventualSortOrder = i * 4;
            //hand[i].SetSortOrder(i * 4); //j
        }
    }

    // The TakeTurn() function enables the AI of the computer Players
    public void TakeTurn()
    {
        Utils.tr("Player.TakeTurn");

        // Don't need to do anything if this is the human player.
        if (type == PlayerType.human) return;

        Bartok.S.phase = TurnPhase.waiting;
        CardBartok cb;
        
        // If this is an AI player, need to make a choice about what to play
        // Find valid plays
        List<CardBartok> validCards = new List<CardBartok>();
        // b
        foreach (CardBartok tCB in hand)
        {
            if (Bartok.S.ValidPlay(tCB))
            {
                validCards.Add(tCB);
            }
        }
        // If there are no valid cards
        if (validCards.Count == 0)
        {
            // c
            // ...then draw a card
            cb = AddCard(Bartok.S.Draw());
            cb.callbackPlayer = this;
            // e
            return;
        }
        // So, there is a card or more to play, so pick one
        cb = validCards[Random.Range(0, validCards.Count)];
        // d
        RemoveCard(cb);
        Bartok.S.MoveToTarget(cb);
        cb.callbackPlayer = this;
        // e
    }
    public void CBCallback(CardBartok tCB)
    {
        Utils.tr("Player.CBCallback()", tCB.name, "Player " + playerNum);
        // The card is done moving, so pass the turn
        Bartok.S.PassTurn();
    }
}
