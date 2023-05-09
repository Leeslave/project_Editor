using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDataDebugger : MonoBehaviour
{
    public string debugSaveData;

    public bool isCreateNewGameData;
    public bool isLoadPlayerData;

    [SerializeField]
    private Date date;
    [SerializeField]
    private List<string> todayWorks;
    [SerializeField]
    private List<int> stageNumber;

    private void Start()
    {
        if (isLoadPlayerData)
            PlayerDataManager.Instance.LoadPlayerData(debugSaveData);
        if (isCreateNewGameData)
            GameDataManager.Instance.CreateGameData();
    }

    private void LateUpdate() {
        if (GameDataManager.Instance.todayData != null)
        {
            date = GameDataManager.Instance.todayData.date;
            todayWorks = GameDataManager.Instance.todayData.workData.Keys.ToList();
            stageNumber = GameDataManager.Instance.todayData.workData.Values.ToList();
        }
    }
}
