using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Manager for a local file where all achievements are stored.
/// Allows saving, loading, and clearing file.
/// </summary>
public static class SaveGameManager {

    // save data into file
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedataSpace.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        Achievement[] achievemets = AchievementManager.GetAchievements();

        formatter.Serialize(stream, achievemets);
        stream.Close();
    }

    // load data from file
    public static Achievement[] Load()
    {
        string path = Application.persistentDataPath + "/savedataSpace.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Achievement[] achievements = formatter.Deserialize(stream) as Achievement[];
            stream.Close();
            return achievements;
        }
        else
        {
            Debug.LogError("SaveData not found in " + path);
            return null;
        }
    }

    // clear data in file
    public static void Delete()
    {
        string path = Application.persistentDataPath + "/savedataSpace.dat";
        // check if file exists
        if (!File.Exists(path))
        {
            Debug.Log( "no " + "/savedata.dat" + " file exists" );
        }
        else
        {
            Debug.Log("/savedata.dat" + " file exists, deleting..." );

            File.Delete(path);
        }
    }
}
