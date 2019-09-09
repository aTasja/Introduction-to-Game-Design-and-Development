using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetHighScore : MonoBehaviour {

	// Update is called once per frame
	public void ResetHighScoreButtonHandler() {
        PlayerPrefs.SetInt("HighScore", 0);
        HighScore.score = 0;

        AudioManager.Play(AudioClipName.Reset);
    }
}
