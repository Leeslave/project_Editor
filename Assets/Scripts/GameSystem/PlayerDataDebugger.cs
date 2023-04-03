using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataDebugger : MonoBehaviour
{
    public string debugSaveData;

    private void Awake()
    {
        PlayerDataManager.LoadPlayerData(debugSaveData);
    }
}
