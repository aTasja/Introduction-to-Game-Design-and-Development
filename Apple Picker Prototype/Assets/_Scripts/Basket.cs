using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basket : MonoBehaviour {
    [Header("Set Dinamically")]
    public Text scoreGT;
    public int scoreToComplicate;

    public static bool HardScore;
    public static bool SprayCatched;

    private void Start()
    {
        // Find a reference to the ScoreCounter GameObject
        GameObject scoreGO = GameObject.Find("ScoreCounter"); 

        // Get the Text Component of that GameObject
        scoreGT = scoreGO.GetComponent<Text>(); 

        // Set the starting number of points to 0
        scoreGT.text = "0";

        // set HardScore and SprayCatched booleans to false
        HardScore = false;
        SprayCatched = false;
    }


    // Update is called once per frame
    void Update () {
        // Get the current screen position of the mouse from Input
        Vector3 mousePos2D = Input.mousePosition;

        // The Camera's z position sets how far to push the mouse into 3
        mousePos2D.z = - Camera.main.transform.position.z;

        // Convert the point from 2D screen space into 3D game world space
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Move the x position of this Basket to the x position of the Mouse
        Vector3 pos = this.transform.position;
        pos.x = mousePos3D.x;
        this.transform.position = pos;
    }

    void OnCollisionEnter(Collision coll)
    { 
        // Find out what hit this basket
        GameObject collidedWith = coll.gameObject; 
        if (collidedWith.tag == "Apple")
        { 
            Destroy(collidedWith);

            AudioManager.Play(AudioClipName.Catch);

            // Parse the text of the scoreGT into an int
            int score = int.Parse(scoreGT.text);
            // Add points for catching the apple
            score += 1;
            // Convert the score back to a string and display it
            scoreGT.text = score.ToString();

            if (score > HighScore.score)
            {
                HighScore.score = score;
            }

            if(score > scoreToComplicate)
                HardScore = true;
            
        }
        if(collidedWith.tag == "Worm")
        {
            Destroy(collidedWith);
            // Get a reference to the ApplePicker component of Main Camera
            ApplePicker apScript = Camera.main.GetComponent<ApplePicker>();
            // Call the public AppleDestroyed() method of apScript
            apScript.AppleDestroyed();
        }
        if(collidedWith.tag == "Spray")
        {
            Destroy(collidedWith);

            AudioManager.Play(AudioClipName.Aerosol);

            SprayCatched = true;
            
        }
    }
}
