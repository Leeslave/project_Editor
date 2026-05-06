using GameService;
using System;
using UnityEngine;
using Utility;

public class DayController : ServiceBase<IDayService>, IDayService
{
    /**
     * 게임내 시간대 컨트롤러
     * - 데이터를 통해서 관리
     */
    
    [SerializeField] private int time;
    [SerializeField] private DailyData data;
    
    public TimeData TimeData => data.dayTimes[time];
    public event Action<int, TimeData> OnTimeChanged;

    public int Time
    {
        get => time;
        set
        {
            time = value;
            EditorLogger.Log($"Time Changed {time}");
            OnTimeChanged?.Invoke(time, TimeData);
        }
    }
    
    public void Init(DailyData newData)
    {
        // 데이터 로드 및 시간 초기화
        data = newData;
        Time = 0;
    }


    /// <summary>
    /// 해당 날짜 정보 불러오기
    /// </summary>
    /// <returns>날짜 정보</returns>
    public Date GetDateInfo()
    {
        return data.date;
    }
}
