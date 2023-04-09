using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataDebugger : MonoBehaviour
{
    public string debugSaveData;

    public string date;
    public int time;
    public string location;
    public int renown;

    private void Awake()
    {
        PlayerDataManager.LoadPlayerData(debugSaveData);
    }

    private void Update()
    {
        date = $"제국력 {PlayerPrefs.GetInt("Year")}년 {PlayerPrefs.GetInt("Month")}/{PlayerPrefs.GetInt("Day")}";
        time = PlayerPrefs.GetInt("Time");
        location = PlayerPrefs.GetString("Location");
        renown = PlayerPrefs.GetInt("Renown");
    }
}
