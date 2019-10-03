using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wyrd
{ // Wyrd does not extend MonoBehaviour
    public string str; // A string representation of the word
    public List<Letter> letters = new List<Letter>();
    public bool found = false; // True if the player has found this word
    
    // A property to set visibility of the 3D Text of each Letter
    public bool visible
    {
        get
        {
            if (letters.Count == 0) return (false);
            return (letters[0].visible);
        }
        set
        {
            foreach (Letter l in letters)
            {
                l.visible = value;
            }
        }
    }

    // A property to set the rounded rectangle color of each Letter
    public Color color
    {
        get
        {
            if (letters.Count == 0) return (Color.black);
            return (letters[0].color);
        }
        set
        {
            foreach (Letter l in letters)
            {
                l.color = value;
            }
        }
    }

    // Adds a Letter to letters
    public void Add(Letter l)
    {
        letters.Add(l);
        str += l.c.ToString();
    }


}