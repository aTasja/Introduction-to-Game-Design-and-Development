using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // We'll be using LINQ
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameMode
{
    preGame, // Before the game starts
    loading, // The word list is loading and being parsed
    makeLevel, // The individual WordLevel is being created
    levelPrep, // The level visuals are Instantiated
    inLevel // The level is in progress
}

public class WordGame : MonoBehaviour
{
    static public WordGame S; // Singleton

    [Header("Set in Inspector")]
    public GameObject prefabLetter;
    public GameObject addNewWord;  //dialog about add new word
    public Rect wordArea = new Rect(-24, 19, 48, 28);
    public float letterSize = 1.5f;
    public bool showAllWyrds = true;
    public float bigLetterSize = 4f;
    public Color bigColorDim = new Color(0.8f, 0.8f, 0.8f);
    public Color bigColorSelected = new Color(1f, 0.9f, 0.7f);
    public Vector3 bigLetterCenter = new Vector3(0, -16, 0);
    public Color[] wyrdPalette;


    [Header("Set Dynamically")]
    public GameMode mode = GameMode.preGame;
    public WordLevel currLevel;
    public List<Wyrd> wyrds;
    public List<Letter> bigLetters;
    public List<Letter> bigLettersActive;
    public string testWord;
    public List<string> wordsToAdd;
    public GameObject listNewWordsGO;
    //private string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private string upperCase = "АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

    private Transform letterAnchor, bigLetterAnchor;

    void Awake()
    {
        S = this; // Assign the singleton
        letterAnchor = new GameObject("LetterAnchor").transform;
        bigLetterAnchor = new GameObject("BigLetterAnchor").transform;
    }

    void Start()
    {
        mode = GameMode.loading;
        // Call the static Init() method of WordList
        WordList.INIT();
    }

    // Called by the SendMessage() command from WordList
    public void WordListParseComplete()
    {
        mode = GameMode.makeLevel;
        // Make a level and assign it to currLevel, the current WordLevel
        currLevel = MakeWordLevel();
    }

    public WordLevel MakeWordLevel(int levelNum = -1)
    {
        WordLevel level = new WordLevel();

        if (levelNum == -1)
        {
            // Pick a random level
            level.longWordIndex = Random.Range(0, WordList.LONG_WORD_COUNT);
        }
        else
        {
            // This will be added later in the chapter
        }

        level.levelNum = levelNum;
        level.word = WordList.GET_LONG_WORD(level.longWordIndex);
        level.charDict = WordLevel.MakeCharDict(level.word);
        StartCoroutine(FindSubWordsCoroutine(level));

        return (level);
    }
    
    // A coroutine that finds words that can be spelled in this level
    public IEnumerator FindSubWordsCoroutine(WordLevel level)
    {
        level.subWords = new List<string>();
        string str;
        List<string> words = WordList.GET_WORDS();
        // Iterate through all the words in the WordList
        for (int i = 0; i < WordList.WORD_COUNT; i++)
        {
            str = words[i];
            // Check whether each one can be spelled using level.charDict
            if (WordLevel.CheckWordInLevel(str, level))
            {
                level.subWords.Add(str);
            }
            // Yield if we've parsed a lot of words this frame
            if (i % WordList.NUM_TO_PARSE_BEFORE_YIELD == 0)
            {
                // yield until the next frame
                yield return null;
            }
        }

        if (level.subWords.Count < 30)
        {
            Debug.Log("МАЛО СЛОВ");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else {
            level.subWords.Sort();
            level.subWords = SortWordsByLength(level.subWords).ToList();

            // The coroutine is complete, so call SubWordSearchComplete()
            SubWordSearchComplete();
        }


        
    }

    // Use LINQ to sort the array received and return a copy // f
    public static IEnumerable<string> SortWordsByLength(IEnumerable<string> ws) { 
        ws = ws.OrderBy(s => s.Length);
        return ws;
    }

    public void SubWordSearchComplete()
    {
        mode = GameMode.levelPrep;
        Layout(); // Call the Layout() function once WordSearch is done
    }

    void Layout()
    {
        // Place the letters for each subword of currLevel on screen
        wyrds = new List<Wyrd>();

        // Declare a lot of local variables that will be used in this method
        GameObject go;
        Letter lett;
        string word;
        Vector3 pos;
        float left = 0;
        float columnWidth = 3;
        char c;
        Color col;
        Wyrd wyrd;

        // Determine how many rows of Letters will fit on screen
        int numRows = Mathf.RoundToInt(wordArea.height / letterSize);

        // Make a Wyrd of each level.subWord
        for (int i = 0; i < currLevel.subWords.Count; i++)
        {
            wyrd = new Wyrd();
            word = currLevel.subWords[i];
            // if the word is longer than columnWidth, expand it
            columnWidth = Mathf.Max(columnWidth, word.Length);
            // Instantiate a PrefabLetter for each letter of the word
            for (int j = 0; j < word.Length; j++)
            {
                c = word[j]; // Grab the jth char of the word
                go = Instantiate<GameObject>(prefabLetter);
                go.transform.SetParent(letterAnchor);
                lett = go.GetComponent<Letter>();
                lett.c = c; // Set the c of the Letter
                // Position the Letter
                pos = new Vector3(wordArea.x + left + j * letterSize, wordArea.y, 0);
                // The % here makes multiple columns line up
                pos.y -= (i % numRows) * letterSize;

                // Move the lett immediately to a position above the screen
                lett.posImmediate = pos + Vector3.up * (20 + i % numRows);

                lett.pos = pos; // You'll add more code around this line later

                // Increment lett.timeStart to move wyrds at different times
                lett.timeStart = Time.time + i * 0.05f;

                go.transform.localScale = Vector3.one * letterSize;
                wyrd.Add(lett);
            }

            if (showAllWyrds) wyrd.visible = true;

            // Color the wyrd based on length
            wyrd.color = wyrdPalette[word.Length - WordList.WORD_LENGTH_MIN];
            wyrds.Add(wyrd);

            // If we've gotten to the numRows(th) row, start a new column
            if (i % numRows == numRows - 1)
            {
                left += (columnWidth + 0.5f) * letterSize;
            }
        }
        // Place the big letters
        // Initialize the List<>s for big Letters
        bigLetters = new List<Letter>();
        bigLettersActive = new List<Letter>();
        // Create a big Letter for each letter in the target word
        for (int i = 0; i < currLevel.word.Length; i++)
        {
            // This is similar to the process for a normal Letter
            c = currLevel.word[i];
            go = Instantiate<GameObject>(prefabLetter);
            go.transform.SetParent(bigLetterAnchor);
            lett = go.GetComponent<Letter>();
            lett.c = c;
            go.transform.localScale = Vector3.one * bigLetterSize;
            // Set the initial position of the big Letters below screen
            pos = new Vector3(0, -100, 0);

            lett.posImmediate = pos;

            lett.pos = pos; // You'll add more code around this line later

            // Increment lett.timeStart to have big Letters come in last
            lett.timeStart = Time.time + currLevel.subWords.Count * 0.05f;
            lett.easingCuve = Easing.Sin + "-0.18"; // Bouncy easing

            col = bigColorDim;
            lett.color = col;
            lett.visible = true; // This is always true for big letters
            lett.big = true;
            bigLetters.Add(lett);
        }
        // Shuffle the big letters
        bigLetters = ShuffleLetters(bigLetters);
        // Arrange them on screen
        ArrangeBigLetters();
        // Set the mode to be in-game
        mode = GameMode.inLevel;
    }

    // This method shuffles a List<Letter> randomly and returns the result
    List<Letter> ShuffleLetters(List<Letter> letts)
    {
        List<Letter> newL = new List<Letter>();
        int ndx;
        while (letts.Count > 0)
        {
            ndx = Random.Range(0, letts.Count);
            newL.Add(letts[ndx]);
            letts.RemoveAt(ndx);
        }
        return (newL);
    }

    // This method arranges the big Letters on screen
    void ArrangeBigLetters()
    {
        // The halfWidth allows the big Letters to be centered
        float halfWidth = ((float)bigLetters.Count) / 2f - 0.5f;
        Vector3 pos;
        for (int i = 0; i < bigLetters.Count; i++)
        {
            pos = bigLetterCenter;
            pos.x += (i - halfWidth) * bigLetterSize;
            bigLetters[i].pos = pos;
        }
        // bigLettersActive
        halfWidth = ((float)bigLettersActive.Count) / 2f - 0.5f;
        for (int i = 0; i < bigLettersActive.Count; i++)
        {
            pos = bigLetterCenter;
            pos.x += (i - halfWidth) * bigLetterSize;
            pos.y += bigLetterSize * 1.25f;
            bigLettersActive[i].pos = pos;
        }
    }

    
    void Update()
    {
        // Declare a couple useful local variables
        Letter ltr;
        char c;
        switch (mode)
        {
            case GameMode.inLevel:
                // Iterate through each char input by the player this frame
                foreach (char cIt in Input.inputString)
                {
                    // Shift cIt to UPPERCASE
                    c = System.Char.ToUpperInvariant(cIt);
                    
                    // Check to see if it's an uppercase letter
                    if (upperCase.Contains(c))
                    { // Any uppercase letter
                      // Find an available Letter in bigLetters with this char
                        ltr = FindNextLetterByChar(c);
                        // If a Letter was returned
                        if (ltr != null)
                        {
                            // ... then add this char to the testWord and move
                            // the returned big Letter to bigLettersActive
                            testWord += c.ToString();
                            // Move it from the inactive to the active List<>
                            bigLettersActive.Add(ltr);
                            bigLetters.Remove(ltr);
                            ltr.color = bigColorSelected; // Make it look active
                            ArrangeBigLetters(); // Rearrange the big Letters
                        }
                    }
                    if (c == '\b')
                    { // Backspace
                      // Remove the last Letter in bigLettersActive
                        if (bigLettersActive.Count == 0) return;
                        if (testWord.Length > 1)
                        {
                            // Clear the last char of testWord
                            testWord = testWord.Substring(0, testWord.Length -
                            1);
                        }
                        else
                        {
                            testWord = "";
                        }
                        ltr = bigLettersActive[bigLettersActive.Count - 1];
                        // Move it from the active to the inactive List<>
                        bigLettersActive.Remove(ltr);
                        bigLetters.Add(ltr);
                        ltr.color = bigColorDim; // Make it the inactive color
                        ArrangeBigLetters(); // Rearrange the big Letters
                    }
                    if (c == '\n' || c == '\r')
                    { // Return/Enter macOS/Windows
                      // Test the testWord against the words in WordLevel
                        CheckWord();
                    }
                    if (c == ' ')
                    { // Space
                      // Shuffle the bigLetters
                        bigLetters = ShuffleLetters(bigLetters);
                        ArrangeBigLetters();
                    }
                }
                break;
        }
    }
    // This finds an available Letter with the char c in bigLetters.
    // If there isn't one available, it returns null.
    Letter FindNextLetterByChar(char c)
    {
        // Search through each Letter in bigLetters
        foreach (Letter ltr in bigLetters)
        {
            // If one has the same char as c
            if (ltr.c == c)
            {
                // ...then return it
                return (ltr);
            }
        }
        return (null); // Otherwise, return null
    }


    public void CheckWord()
    {
        // Test testWord against the level.subWords
        string subWord;
        bool foundTestWord = false;
        // Create a List<int> to hold the indices of other subWords that are
        // contained within testWord
        List<int> containedWords = new List<int>();
        // Iterate through each word in currLevel.subWords
        for (int i = 0; i < currLevel.subWords.Count; i++)
        {
            // Check whether the Wyrd has already been found
            if (wyrds[i].found)
            {
                continue;
            }
            subWord = currLevel.subWords[i];
            // Check whether this subWord is the testWord or is contained in it
            if (string.Equals(testWord, subWord))
            {
                HighlightWyrd(i);
                ScoreManager.SCORE(wyrds[i], 1); // Score the testWord
                foundTestWord = true;
            }
            else if (testWord.Contains(subWord))
            {
                //containedWords.Add(i);
            }
            
        }

        if (foundTestWord)
        { // If the test word was found in subWords
          // ...then highlight the other words contained in testWord
            int numContained = containedWords.Count;
            int ndx;
            // Highlight the words in reverse orderре
            for (int i = 0; i < containedWords.Count; i++)
            {
                ndx = numContained - i - 1;
                HighlightWyrd(containedWords[ndx]);
                ScoreManager.SCORE(wyrds[containedWords[ndx]], i + 2);
            }
            // Clear the active big Letters regardless of whether testWord was valid
            ClearBigLettersActive();
        }
        else 
        {// instantiate dialog about add new word or not
            bool foundInFounded = false;
            foreach(Wyrd wyrd in wyrds)
            {
                if (wyrd.str == testWord)
                    foundInFounded = true;
            }
            foreach(string w in wordsToAdd)
            {
                if (testWord == w)
                    foundInFounded = true;
            }
            if (!foundInFounded)
                Instantiate(addNewWord);
            ClearBigLettersActive();


        }

        
    }


    // Highlight a Wyrd
    void HighlightWyrd(int ndx)
    {
        // Activate the subWord
        wyrds[ndx].found = true; // Let it know it's been found
                                 // Lighten its color
        wyrds[ndx].color = (wyrds[ndx].color + Color.white) / 2f;
        wyrds[ndx].visible = true; // Make its 3D Text visible
    }

    // Remove all the Letters from bigLettersActive
    void ClearBigLettersActive()
    {
        testWord = ""; // Clear the testWord
        foreach (Letter ltr in bigLettersActive)
        {
            bigLetters.Add(ltr); // Add each Letter to bigLetters
            ltr.color = bigColorDim; // Set it to the inactive color
        }
        bigLettersActive.Clear(); // Clear the List<>
        ArrangeBigLetters(); // Rearrange the Letters on screen
    }


 
}