
using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

public class MockDataController : MonoBehaviour, IDataService
{
    /**
     * 디버그용 Data 정보 설정
     * - 유니티 에디터상에서 데이터 설정
     */

    public int dayIndex;
    
    public DailyData testData;
    
    public DailyData Data => testData;

    private void Awake()
    {
        ServiceProvider.Register<IDataService>(this);
    }
    

    public void Init()
    {
        LoadDay();
    }
    
    public event Action<int> OnDataChanged;
    public DailyData LoadDay(int dateIndex = -1)
    {
        OnDataChanged?.Invoke(0);
        return Data;
    }

    public List<Work> GetWorkList()
    {
        return testData.workList;
    }

    [SerializeField] private PlayerData _playerData;
    public PlayerData Player => _playerData;
    
    public DaySave GetDaySave(int dateIndex = -1)
    {
        return Player[dateIndex];
    }

    public void SaveDay(DaySave daySave)
    {
        EditorLogger.Log($"Day Saved! {daySave}");
        _playerData[dayIndex] = daySave;
    }
}
