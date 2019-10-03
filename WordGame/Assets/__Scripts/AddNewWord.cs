using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddNewWord : MonoBehaviour {

    [Header("Set in Inspector")]
    public Text word;
    public GameObject newWords;    

	// Use this for initialization
	void Awake () {
        word.text = WordGame.S.testWord;
	}
	
	public void OnYesEventHandler()
    {
        if (!WordGame.S.wordsToAdd.Contains(word.text))
        {
            WordGame.S.wordsToAdd.Add(word.text);
            Scoreboard.S.score = Scoreboard.S.score + word.text.Length;
            if (WordGame.S.listNewWordsGO == null)
            {
                WordGame.S.listNewWordsGO = Instantiate(newWords);
            }
            WordGame.S.listNewWordsGO.GetComponent<ListNewWords>().UpdateListOfNewWords();
        }
        Destroy(gameObject);
    }

    public void OnNoEventHandler()
    {
        Destroy(gameObject);
    }
}
