using GameService;
using System;
using UnityEngine;
using Utility;

public class DayController : MonoBehaviour, IDayService
{
    /**
     * 게임 날짜 컨트롤러
     * - 데이터를 통해서 관리
     */
    
    [SerializeField] private int day;
    [SerializeField] private int time;
    [SerializeField] private int branch;

    [SerializeField] private DailyData data;
    
    public TimeData TimeData => data.dayTimes[time];

    public event Action<DailyData> OnDayChanged;
    public event Action<TimeData> OnTimeChanged;

    public int Day
    {
        get => day;
        set => ChangeDay(value);
    }

    public int Time
    {
        get => time;
        set
        {
            time = value;
            EditorLogger.Log($"Day Changed {time}");
            OnTimeChanged?.Invoke(TimeData);
        }
    }

    private void Awake()
    {
        GameSystem.Instance.RegisterService(this);
    }

    private void ChangeDay(int newDay)
    {
        // 데이터 확인
        var newData = DataLoader.GetDayData(newDay);
        if (newData == null)
        {
            return;
        }
        data = newData;
        
        // 날짜 설정
        day = newDay;
        EditorLogger.Log($"Day Changed {day}");

        var saveService = GameSystem.Instance.GetService<ISaveService>();
        saveService.SelectDay(newDay);
        
        OnDayChanged?.Invoke(data);
        
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

    void OnDestroy()
    {
        GameSystem.Instance.UnRegister(this);
    }
}
