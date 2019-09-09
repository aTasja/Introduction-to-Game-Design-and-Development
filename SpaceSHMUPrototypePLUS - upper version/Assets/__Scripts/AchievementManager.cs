using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementManager : MonoBehaviour {

    static public AchievementManager S;

    public Achievement[] achievements;
    public GameObject[] achievementBoxes;
    

    public Dictionary<WeaponType, Achievement> ACH_DICT;
    private bool allAchivementsComplete = false;

    // Use this for initialization
    void Start () {

        if (S == null)
        {
            S = this; // Set the Singleton // a
        }
        else
        {
            Debug.LogError("AchievementManager.Awake() - Attempted to assign second AchievementManager.S!");
        }

        if(SaveGameManager.Load() != null)
        {
            achievements = SaveGameManager.Load();
        }
        

        ACH_DICT = new Dictionary<WeaponType, Achievement>();
		for(int i=0; i < achievements.Length; i++)
        {
            Achievement ach = achievements[i];
            ACH_DICT[ach.type] = ach;

            Renderer cubeRend = achievementBoxes[i].transform.Find("Cube").GetComponent<Renderer>();
            cubeRend.material.color = Main.GetWeaponDefinition(ach.type).projectileColor;

            GameObject chMark = achievementBoxes[i].transform.Find("checkedMark").gameObject;
            chMark.SetActive(ach.achieved);
        }
	}

    void checkAchivement(WeaponType type) 
    {
        ACH_DICT[type].achieved = true;
        
        GameObject chMark = achievementBoxes[ACH_DICT[type].serialNumber].transform.Find("checkedMark").gameObject;
        chMark.SetActive(true);
        SaveGameManager.Save();

        
    }

    static public void ClaerAchievements()
    {
        for(int i=0; i<S.achievements.Length; i++)
        {
            S.achievements[i].achieved = false;
            GameObject chMark = S.achievementBoxes[i].transform.Find("checkedMark").gameObject;
            chMark.SetActive(S.achievements[i].achieved);
        }
        SaveGameManager.Delete();
    }

    static public void CheckAchievement(WeaponType type)
    {
        S.checkAchivement(type);
    }

    static public Achievement[] GetAchievements()
    {
        return S.achievements;
    }

    static public bool AllAchivementsComplete
    {
        get
        {
            foreach (Achievement ach in S.achievements)
            {
                if (ach.achieved == false)
                    return false;
            }
            return true;
        }
    }

}

[Serializable]
public class Achievement
{
    public WeaponType type;
    public int serialNumber;
    public bool achieved;
}
