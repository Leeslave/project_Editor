using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private class SaveData
    {
        public static SaveData instance;
        public static Dictionary<string, int> date;
        public static int time;
        public static int renown;

        static SaveData()
        {
            // 게임 상 첫 시작 날짜 : 제국력 17년 12월 13일
            date = new Dictionary<string, int>();
            date.Add("Year", 17);
            date.Add("Month", 12);
            date.Add("Day", 13);

            // 하루 시간대 (0 : 출근 전, 1 : 업무 전, 2 : 업무 후, 3 : 저녁)
            time = 0;

            renown = 0;
        }
    }

    public void InitNewPlayerData()
    {   
        foreach(var key in SaveData.date.Keys)
        {
            PlayerPrefs.SetInt(key, SaveData.date[key]);
        }

        PlayerPrefs.SetInt("Time", SaveData.time);
        PlayerPrefs.SetInt("Renown", SaveData.renown);
    }

    public void SavePlayerData()
    {
        foreach (var key in SaveData.date.Keys)
        {
            SaveData.date[key] = PlayerPrefs.GetInt(key);
        }

        SaveData.time = PlayerPrefs.GetInt("Time");
        SaveData.renown = PlayerPrefs.GetInt("Renown");

        JsonUtility.ToJson(SaveData.instance);
    }
}
