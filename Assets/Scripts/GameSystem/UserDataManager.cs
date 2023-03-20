using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private static class SaveData
    {
        public static Dictionary<string, int> date;
        public static int time;
        public static int renown;

        static SaveData()
        {
            // ���� �� ù ���� ��¥ : ������ 17�� 12�� 13��
            date = new Dictionary<string, int>();
            date.Add("Year", 17);
            date.Add("Month", 12);
            date.Add("Day", 13);

            // �Ϸ� �ð��� (0 : ��� ��, 1 : ���� ��, 2 : ���� ��, 3 : ����)
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

        ;
    }
}
