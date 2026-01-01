using GameData;
using GameService;
using System;
using UnityEngine;

public class DayController : MonoBehaviour, IDayService
{
    /**
     * 게임 날짜 컨트롤러
     */
    [SerializeField] private IDataService dataService;
    
    [SerializeField] private Date dateInfo;
    [SerializeField] private uint date ;
    [SerializeField] private uint time;
        
    public event Action<uint> OnDateChanged;
    public event Action<uint> OnTimeChanged;
    
    public uint Date
    {
        get => date;
        set
        {
            if (date == value)
                return;
            date = value;
            OnDateChanged?.Invoke(value);
        }
    }

    public uint Time
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

    public void Init()
    {
        dataService = ServiceProvider.Get<IDataService>();
    }

    /// <summary>
    /// 해당 날짜 정보 불러오기
    /// </summary>
    /// <returns></returns>
    public Date GetDateInfo()
    {
        return dateInfo;
    }
}
