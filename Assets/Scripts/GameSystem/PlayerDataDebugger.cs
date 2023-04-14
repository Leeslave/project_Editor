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
        PlayerDataManager.Instance.LoadPlayerData(debugSaveData);
    }

    private void Update()
    {
        date = $"제국력 {PlayerDataManager.Instance.playerData.date.year}년 {PlayerDataManager.Instance.playerData.date.month}/{PlayerDataManager.Instance.playerData.date.day}";
        time = PlayerDataManager.Instance.playerData.time;
        location = PlayerDataManager.Instance.playerData.location;
        renown = PlayerDataManager.Instance.playerData.renown;
    }
}
