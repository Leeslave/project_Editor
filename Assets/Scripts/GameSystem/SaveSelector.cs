using GameService;
using System.Collections.Generic;
using UnityEngine;

public class SaveSelector : MonoBehaviour
{
    /**
     * 첫 로드시 세이브 불러와 UI 생성 및 이벤트 연결
    */
    [SerializeField]
    private Transform panel;
    private readonly List<Transform> _dayPanels = new();
    
    [Header("UI Prefab")]
    [SerializeField] private GameObject dayPanelPrefab;
    [SerializeField] private GameObject dayButtonPrefab;

    private void Start()
    {
        SaveService save = GameSystem.SaveService;
        if (save)
        {
            save.Init();
            Init(save.GetPlayerData());
        }
    }
    
    private void Init(PlayerData playerData)
    {
        Clear();

        // 날짜 생성 초기화
        int dayCount = 99;
        Transform dayPanel = Instantiate(dayPanelPrefab, panel).transform;    // 0일차 기본 생성
        _dayPanels.Add(dayPanel);
        
        foreach (DaySave save in playerData)
        {
            // 날짜별로 패널 생성
            if (save.dayID > dayCount)
            {
                dayPanel = Instantiate(dayPanelPrefab, panel).transform;
                _dayPanels.Add(dayPanel);
                dayCount += 100;
            }
            GameObject newWindow = Instantiate(dayButtonPrefab, dayPanel);
            
            // 날짜 창 설정
            SaveWindow saveWindow = newWindow.GetComponent<SaveWindow>();
            saveWindow.Init(save);
        }

        // 새로 시작
        if (playerData.Count == 0)
        {
            GameObject newWindow = Instantiate(dayButtonPrefab, dayPanel);
            
            // 새로 시작 창 생성
            SaveWindow saveWindow = newWindow.GetComponent<SaveWindow>();
            saveWindow.Init(new DaySave());
        }
    }

    private void Clear()
    {
        if (_dayPanels.Count <= 0)
        {
            return;
        }

        foreach (var obj in _dayPanels)
        {
            Destroy(obj.gameObject);
        }
    }
}
