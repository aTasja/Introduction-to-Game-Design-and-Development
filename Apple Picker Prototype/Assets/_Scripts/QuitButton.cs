using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour {

    public void QuikButton_Handler()
    {
        SceneManager.LoadScene("Splash");
    }
}
