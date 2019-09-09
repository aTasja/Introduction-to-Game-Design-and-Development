using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

	public void PlayButton_Handler()
    {
        SceneManager.LoadScene("_Scene_0");
    }
}
