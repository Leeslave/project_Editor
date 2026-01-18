
using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class DataController : IDataService
{
    /**
     * 데이터파일 기반 Date 정보 로더
     * - 해당 index에 따라 데이터 로드 및 제공
     */
    private int _index;
    
    public DailyData Data { get; private set; }
    
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

    /// <summary>
    /// 날짜 로드
    /// </summary>
    /// <param name="dateIndex">해당하는 날짜 인덱스 (default -1 : 현재 날짜)</param>
    public Date LoadDay(int dateIndex = -1)
    {
        if (dateIndex >= 0)
        {
            _index = dateIndex;
        }

        Data = DataLoader.GetDayData(_index);    // Exception 주의
        
        OnDataChanged?.Invoke(_index);
        
        return GetDateInfo();
    }

    /// <summary>
    /// 오늘 날짜 상세 정보 반환
    /// </summary>
    /// <returns>날짜 상세 정보</returns>
    public Date GetDateInfo()
    {
        return Data.date;
    }

    /// <summary>
    /// 오늘 시작 위치 반환
    /// </summary>
    /// <returns>시작 좌표 enum</returns>
    public WorldVector GetStartLocation()
    {
        return Data.startLocation;
    }

    /// <summary>
    /// 오늘 날짜 시간대별 데이터 리스트 반환
    /// </summary>
    /// <returns>시간대별 데이터 리스트 반환 (List[4])</returns>
    public List<TimeData> GetTimeData()
    {
        return Data.dayTimes.ToList();
    }

    /// <summary>
    /// 오늘 날짜 업무 정보 반환
    /// </summary>
    /// <returns>날짜 정보 리스트 반환</returns>
    public List<Work> GetWorkList()
    {
        return Data.workList;
    }
}
