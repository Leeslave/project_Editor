
using GameService;
using System;
using System.Collections.Generic;

public sealed class DataController : IDataService
{
    /**
     * 데이터파일 기반 Date 정보 로더
     * - 해당 index에 따라 데이터 로드 및 제공
     */
    private int _index;
    
    public event Action<int> OnDataChanged;

    public DataController()
    {
        ServiceProvider.Register(this as IDataService);
        
        Init();
    }
    
    /// <summary>
    /// 데이터 로드
    /// </summary>
    /// <remarks>index 값에 해당하는 데이터 로드</remarks>
    public void Init()
    {
        if (_index >= 0)
        {
            LoadDay(_index);
        }
    }
    
    #region DayData
    
    public DailyData Data { get; private set; }

    /// <summary>
    /// 날짜 로드
    /// </summary>
    /// <param name="dateIndex">해당하는 날짜 인덱스 (default -1 : 현재 날짜)</param>
    public DailyData LoadDay(int dateIndex = -1)
    {
        if (dateIndex >= 0)
        {
            _index = dateIndex;
        }

        Data = DataLoader.GetDayData(_index);    // Exception 주의
        
        OnDataChanged?.Invoke(_index);
        
        return Data;
    }
    
    /// <summary>
    /// 오늘 날짜 업무 정보 반환
    /// </summary>
    /// <returns>날짜 정보 리스트 반환</returns>
    public List<Work> GetWorkList()
    {
        return Data.workList;
    }
    
    #endregion

    #region SaveData
    
    public SaveData Save { get; private set; }
    public DaySave GetDaySave(int dateIndex = -1)
    {
        return Save.saveList[dateIndex];
    }

    #endregion
}
