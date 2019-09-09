using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ApplePicker : MonoBehaviour {

    [Header("Set in Inspector")]
    public GameObject basketPrefab;
    public GameObject treePrefab;
    public int numBaskets = 3;
    public int scoreForHardMode = 5;
    public float basketBottomY = -13.5f;
    public float basketSpacingY = 2f;
    public List<GameObject> basketList;

    // Use this for initialization
    void Start () {

        AudioManager.Play(AudioClipName.NewGame);

        basketList = new List<GameObject>();
        for (int i = 0; i < numBaskets; i++)
        {
            GameObject tBasketGO = Instantiate<GameObject>(basketPrefab);
            Vector3 pos = Vector3.zero;
            pos.y = basketBottomY + (basketSpacingY * i);
            tBasketGO.transform.position = pos;
            basketList.Add(tBasketGO);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AppleDestroyed()
    {
        // Destroy all of the falling items
        GameObject[] tAppleArray = GameObject.FindGameObjectsWithTag("Apple");
        foreach (GameObject tGO in tAppleArray)
        {
            Destroy(tGO);
        }
        GameObject[] tWormArray = GameObject.FindGameObjectsWithTag("Worm");
        foreach (GameObject tGO in tAppleArray)
        {
            Destroy(tGO);
        }
        GameObject[] tSprayArray = GameObject.FindGameObjectsWithTag("Spray");
        foreach (GameObject tGO in tAppleArray)
        {
            Destroy(tGO);
        }

        // Destroy one of the baskets
        // Get the index of the last Basket in basketList
        int basketIndex = basketList.Count - 1;

        // Get a reference to that Basket GameObject
        GameObject tBasketGO = basketList[basketIndex];

        // Remove the Basket from the list and destroy the GameObject
        basketList.RemoveAt(basketIndex);
        Destroy(tBasketGO);

        AudioManager.Play(AudioClipName.Drop);

        // If there are no Baskets left, restart the game
        if (basketList.Count == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("_Scene_0");
        }


    }
}
