using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataDebugger : MonoBehaviour
{
    public UserDataManager userDataManager;
    public string debugSaveData;



    private void Awake()
    {
        if (userDataManager == null)
            userDataManager = GameObject.FindObjectOfType<UserDataManager>();

        userDataManager.InitNewPlayerData();

        userDataManager.SavePlayerData();
    }
}
