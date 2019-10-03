using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListNewWords : MonoBehaviour {

    public Text listText;

	// Use this for initialization
	public void UpdateListOfNewWords() {
        listText.text = "";
        foreach (string w in WordGame.S.wordsToAdd)
        {
            listText.text += w + ", ";
        }
    }

}
