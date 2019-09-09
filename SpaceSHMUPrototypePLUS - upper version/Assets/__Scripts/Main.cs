using System.Collections; // Required for Arrays & other Collections
using System.Collections.Generic; // Required to use Lists or Dictionaries
using UnityEngine; // Required for Unity
using UnityEngine.SceneManagement; // For loading & reloading of scenes
using UnityEngine.UI;


public class Main : MonoBehaviour
{
    static public Main S; // A singleton for Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]

    public GameObject[] prefabEnemies; // Array of Enemy prefabs
    public float enemySpawnPerSecond = 0.5f; // #Enemies/second
    public float enemyDefaultPadding = 1.5f; // Padding for position
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public GameObject startTitle;
    public Button playButton;
    public GameObject missionComplete;
    public GameObject pressResetToTryAgain;



    public WeaponType[] powerUpFrequency = new WeaponType[]
                                                {WeaponType.blaster, WeaponType.blaster,
                                                 WeaponType.spread, WeaponType.shield};

    private BoundsCheck bndCheck;
    private GameObject tempPressResetToTryAgain;
    private GameObject tempmissionComplete;


    public enum GameState
    {
        start,
        level,
        gameOver,
    }

    private GameState _gameState;

    void Awake()
    {
        S = this;
        // Set bndCheck to reference the BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        { // b
            WEAP_DICT[def.type] = def;
        }



    }

    private void Start()
    {
        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)

        _gameState = GameState.start;

        startTitle.SetActive(true);

        if (AchievementManager.AllAchivementsComplete)
        {
            AudioManager.Play(AudioClipName.MissionCompleted);
            playButton.enabled = false;
            StartCoroutine("startTryAgain");
        }
    }

    IEnumerator startTryAgain()
    {
        tempmissionComplete = Instantiate(missionComplete);
        yield return new WaitForSeconds(1.5f);
        tempmissionComplete.GetComponent<Animator>().speed = 0;

        tempPressResetToTryAgain = Instantiate(pressResetToTryAgain);
        yield return new WaitForSeconds(1.5f);
        tempPressResetToTryAgain.GetComponent<Animator>().speed = 0;
    }

    // Update is called once per frame
    void Update()
    {

        // pause game on escape key
        if (_gameState == GameState.level)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //CrossPlatformInputManager.GetButtonDown("escape"))
            {
                AudioManager.Play(AudioClipName.ButtonClick);
                SceneManager.LoadScene("_Scene_0");
            }
        }


    }

    public void ShipDestroyed(Enemy e)
    { // Potentially generate a PowerUp

        AudioManager.Play(AudioClipName.Boom);
        powerUpFrequency[0] = Hero.GetZeroWeapon();

        if (Random.value <= e.powerUpDropChance) // Random.value - property that generates a random float between 0(inclusive) and 1(inclusive)
        { // Choose which PowerUp to pick
          // Pick one from the possibilities in powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length); // e
            WeaponType puType = powerUpFrequency[ndx];

            // Spawn a PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();

            // Set it to the proper WeaponType
            pu.SetType(puType); // f
            // Set it to the position of the destroyed ship
            pu.transform.position = e.transform.position;
        }
    }

    

    public void SpawnEnemy()
    {

        if (_gameState == GameState.level)
        {
            // Pick a random Enemy prefab to instantiate
            int ndx = Random.Range(0, prefabEnemies.Length); // b
            GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]); // c

            // Position the Enemy above the screen with a random x position
            float enemyPadding = enemyDefaultPadding; // d

            if (go.GetComponent<BoundsCheck>() != null)
            { // e
                enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
            }

            // Set the initial position for the spawnedEnemy // f
            Vector3 pos = Vector3.zero;
            float xMin = -bndCheck.camWidth + enemyPadding;
            float xMax = bndCheck.camWidth - enemyPadding;
            pos.x = Random.Range(xMin, xMax);
            pos.y = bndCheck.camHeight + enemyPadding;
            go.transform.position = pos;

            // Invoke SpawnEnemy() again
            Invoke("SpawnEnemy", 1f / enemySpawnPerSecond); // g
        }

    }

    public void DelayedRestart(float delay, bool win)
    {
        // Invoke the Restart() method in delay seconds
        AudioManager.Stop();

        Invoke("Restart", delay);
        if (!win)
        {
            AudioManager.Play(AudioClipName.Restart);
        }
        else
        { 
            
            AudioManager.Play(AudioClipName.GameOver);
        }
    }

    public void Restart()
    {
        // Reload _Scene_0 to restart the game
        SceneManager.LoadScene("_Scene_0");
    }

    /// <summary>
    /// Static function that gets a WeaponDefinition from the WEAP_DICT static
    /// protected field of the Main class.
    /// </summary>
    /// <returns>The WeaponDefinition or, if there is no WeaponDefinition with
    /// the WeaponType passed in, returns a new WeaponDefinition with a
    /// WeaponType of none..</returns>
    /// <param name="wt">The WeaponType of the desired WeaponDefinition</param>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    { // a
        // Check to make sure that the key exists in the Dictionary
        // Attempting to retrieve a key that didn't exist, would throw an error,
        // so the following if statement is important.
        if (WEAP_DICT.ContainsKey(wt))
        { // b
            return (WEAP_DICT[wt]);
        }
        // This returns a new WeaponDefinition with a type of WeaponType.none,
        // which means it has failed to find the right WeaponDefinition
        return (new WeaponDefinition()); // c
    }

    public void OnPlayButtonHandler()
    {
        AudioManager.Play(AudioClipName.ButtonClick);
        _gameState = GameState.level;
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond); // a
        startTitle.SetActive(false);


    }

    public void OnResetButtonHandler()
    {
        AudioManager.Play(AudioClipName.ButtonClick);
        AchievementManager.ClaerAchievements();
        AudioManager.Stop();

        if (playButton.enabled == false)
        {
            playButton.enabled = true;
            Destroy(tempPressResetToTryAgain);
            Destroy(tempmissionComplete);
        }

        //SceneManager.LoadScene("_Scene_0");


    }

    

    static public GameState GAME_STATE
    {
        get { return S._gameState; }
    }

}