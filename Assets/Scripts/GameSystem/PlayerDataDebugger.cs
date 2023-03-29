using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataDebugger : MonoBehaviour
{
    public PlayerDataManager userDataManager;
    public string debugSaveData;



    private void Awake()
    {
        if (userDataManager == null)
            userDataManager = GameObject.FindObjectOfType<PlayerDataManager>();

        userDataManager.LoadPlayerData("debugsave_1");
    }
}
