using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour {

    bool paused;
    Text pausedText;
    public float fadeSpeed;

    private void Start()
    {
        paused = false;
        pausedText = GameObject.Find("PausedText").GetComponent<Text>();
        pausedText.enabled = false;
    }

    // Use this for initialization
    public void PauseButton_Handler() {
        if (!paused)
        {
            Time.timeScale = 0;
            paused = true;
            pausedText.enabled = true;
        }
        else if (paused)
        {
            Time.timeScale = 1;
            paused = false;
            pausedText.enabled = false;
        } 
    }


}
