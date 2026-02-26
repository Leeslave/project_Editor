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
    private IDataService _dataService;
    
    [SerializeField] private int date;
    [SerializeField] private int time;

    [SerializeField] private DailyData data;
    public DailyData Data => data;
    public TimeData TimeData => data.dayTimes[time];
    public event Action<int> OnDateChanged;
    public event Action<int> OnTimeChanged;
    
    public int Date
    {
        get => date;
        set
        {
            if (date == value)
                return;
            date = value;

            // Data 갱신
            data = _dataService.LoadDay(date);
            
            OnDateChanged?.Invoke(value);
            Time = 0;
        }
    }

    public int Time
    {
        get => time;
        set
        {
            time = value;
            EditorLogger.Log($"Day Changed {time}");
            OnTimeChanged?.Invoke(value);
        }
    }

    private void Awake()
    {
        ServiceProvider.Register<IDayService>(this);
    }

    private void Start()
    {
        _dataService = ServiceProvider.Get<IDataService>();
        #if UNITY_EDITOR
        // NOTE: 디버그용 날짜 즉시 로드
        Init();
        #endif
    }

    public void Init()
    {
        Date = 0;
    }

    /// <summary>
    /// 해당 날짜 정보 불러오기
    /// </summary>
    /// <returns>날짜 정보</returns>
    public Date GetDateInfo()
    {
        return data.date;
    }

    public WorldVector GetStartLocation()
    {
        return data.startLocation;
    }
}
