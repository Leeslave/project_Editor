
using GameData;
using GameService;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MockDataController : MonoBehaviour, IDataService
{
    /**
     * 디버그용 Data 정보 설정
     * - 유니티 에디터상에서 데이터 설정
     */

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
    public void LoadDay(int dateIndex = -1)
    {
        OnDataChanged?.Invoke(0);
    }

    public Date GetDateInfo()
    {
        return testData.date;
    }

    public WorldVector GetStartLocation()
    {
        return testData.startLocation;
    }

    public List<TimeData> GetTimeData()
    {
        return new List<TimeData>();
    }

    public List<Work> GetWorkList()
    {
        return new List<Work>();
    }
}
