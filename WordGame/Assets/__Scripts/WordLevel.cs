using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // WordLevels can be viewed & edited in the Inspector
public class WordLevel
{ // WordLevel does NOT extend MonoBehaviour
    public int levelNum;
    public int longWordIndex;
    public string word;

    // A Dictionary<,> of all the letters in word
    public Dictionary<char, int> charDict;

    // All the words that can be spelled with the letters in charDict
    public List<string> subWords;

    // A static function that counts the instances of chars in a string and
    // returns a Dictionary<char,int> that contains this information
    static public Dictionary<char, int> MakeCharDict(string w)
    {
        Dictionary<char, int> dict = new Dictionary<char, int>();
        char c;
        for (int i = 0; i < w.Length; i++)
        {
            c = w[i];
            if (dict.ContainsKey(c))
            {
                dict[c]++;
            }
            else
            {
                dict.Add(c, 1);
            }
        }
        return (dict);
    }

    // This static method checks to see whether the word can be spelled with the
    // chars in level.charDict
    public static bool CheckWordInLevel(string str, WordLevel level)
    {
        Dictionary<char, int> counts = new Dictionary<char, int>();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            // If the charDict contains char c
            if (level.charDict.ContainsKey(c))
            {
                // If counts doesn't already have char c as a key
                if (!counts.ContainsKey(c))
                {
                    // ...then add a new key with a value of 1
                    counts.Add(c, 1);
                }
                else
                {
                    // Otherwise, add 1 to the current value
                    counts[c]++;
                }
                // If this means that there are more instances of char c in str
                // than are available in level.charDict
                if (counts[c] > level.charDict[c])
                {
                    // ... then return false
                    return (false);
                }
            }
            else
            {
                // The char c isn't in level.word, so return false
                return (false);
            }
        }
        return (true);
    }
}