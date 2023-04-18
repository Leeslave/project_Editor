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

    private void Start()
    {
        PlayerDataManager.Instance.LoadPlayerData(debugSaveData);
        GameDataManager.Instance.CreateGameData();
    }
}
