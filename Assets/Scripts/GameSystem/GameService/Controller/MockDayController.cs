using GameService;
using UnityEngine;
using System;

public class MockDayController : MonoBehaviour, IDayService
{
    /**
     * 게임 날짜 컨트롤러
     * - 데이터를 통해서 관리
     */
    
    [SerializeField] private Date dateInfo;
    [SerializeField] private int date ;
    [SerializeField] private int time;
        
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
            OnDateChanged?.Invoke(value);
            Time = 0;
        }
    }

    public int Time
    {
        get => time;
        set
        {
            if (time == value)
                return;
            time = value;
            
            OnTimeChanged?.Invoke(value);
        }
    }

    private void Awake()
    {
        ServiceProvider.Register<IDayService>(this);
    }

    public void Init()
    {
        
    }

    /// <summary>
    /// 해당 날짜 정보 불러오기
    /// </summary>
    /// <returns>날짜 정보</returns>
    public Date GetDateInfo()
    {
        return dateInfo;
    }
}
