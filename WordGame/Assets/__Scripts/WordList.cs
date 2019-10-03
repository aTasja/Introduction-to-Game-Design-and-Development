using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class WordList : MonoBehaviour {

    private static WordList S;
    
    [Header("Set in Inspector")]
    public TextAsset wordListText;
    public int numToParseBeforeYield = 10000;
    public int wordLengthMin = 3;
    public int wordLengthMax = 10;
    [Header("Set Dynamically")]
    public int currLine = 0;
    public int totalLines;
    public int longWordCount;
    public int wordCount;
    // Private fields
    private string[] lines; // b
    private List<string> longWords;
    private List<string> words;

    void Awake()
    {
        S = this; // Set up the WordList Singleton
    }

    public void Init() // This line replaces "void Start()"
    {

        lines = wordListText.text.Split('\n'); // c

        totalLines = lines.Length;
        StartCoroutine(ParseLines()); // d
    }

    static public void INIT()
    { // a
        S.Init();
    }

    // All coroutines have IEnumerator as their return type.
    public IEnumerator ParseLines()
    { // e
        string word;

        // Init the Lists to hold the longest words and all valid words
        longWords = new List<string>(); // f
        words = new List<string>();
        for (currLine = 0; currLine < totalLines; currLine++)
        { // g
            if (lines[currLine].Length < 2) { continue;}
            word = lines[currLine].Substring(0, lines[currLine].Length - 1); // delete space in the end of each word
            
            // If the word is as long as wordLengthMax…
            if (word.Length == wordLengthMax)
            {
                longWords.Add(word); // …then store it in longWords
            }
            // If it's between wordLengthMin and wordLengthMax in length…
            if (word.Length >= wordLengthMin && word.Length <= wordLengthMax)
            {
                words.Add(word); // …then add it to the list of all valid words
            }
            // Determine whether the coroutine should yield
            if (currLine % numToParseBeforeYield == 0)
            { // h
              // Count the words in each list to show parsing progress
                longWordCount = longWords.Count;
                wordCount = words.Count;
                // This yields execution until the next frame
                yield return null; // i
                        // The yield will cause the execution of this method to wait
                        // here while other code executes and then continue from this
                        // point into the next iteration of the for loop.
            }
        }
        longWordCount = longWords.Count;
        wordCount = words.Count;
        Debug.Log(words[wordCount-1]);

        // Send a message to this gameObject to let it know the parse is done
        gameObject.SendMessage("WordListParseComplete");
    }

    // These methods allow other classes to access the private List<string> s // j
    static public List<string> GET_WORDS()
    {
        return (S.words);
    }

    static public string GET_WORD(int ndx)
    {
        return (S.words[ndx]);
    }

    static public List<string> GET_LONG_WORDS()
    {
        return (S.longWords);
    }

    static public string GET_LONG_WORD(int ndx)
    {
        return (S.longWords[ndx]);
    }

    static public int WORD_COUNT
    {
        get { return S.wordCount; }
    }

    static public int LONG_WORD_COUNT
    {
        get { return S.longWordCount; }
    }

    static public int NUM_TO_PARSE_BEFORE_YIELD
    {
        get { return S.numToParseBeforeYield; }
    }
    static public int WORD_LENGTH_MIN
    {
        get { return S.wordLengthMin; }
    }

    static public int WORD_LENGTH_MAX
    {
        get { return S.wordLengthMax; }
    }

   // [MenuItem("Tools/Write file")]
    static public void AddNewWord(string word)
    {
        string path = "Assets/Resources/russianWord.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(word);
        writer.Close();

        //Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path);

        //Print the text from the file
        Debug.Log("<" + word + "> added");
    }

    
}