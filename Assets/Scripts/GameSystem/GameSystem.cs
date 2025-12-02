using System;
using System.Collections.Generic;
using GameData;
using GameService;
using UnityEngine;

public sealed class GameSystem : Singleton<GameSystem>, ISaveService
{
    /**
     * 게임 메인 시스템 로직
     * - 데이터 로드 및 관리
     * - 세이브 관리
     * - 메인씬 로드 (게임 진입)
     */
    private void Init()
    {
        
    }

    public new void Awake()
    {
        base.Awake();
        Init();
    }
    
    
    
    #region SaveManage
    //////// Save 관리 ////////
    
    [SerializeField] private SaveData _saveData;

    public int Renown
    {
        get => _saveData.renown;
        set
        {
            _saveData.renown = value;
            OnRenownChanged?.Invoke(value);
        }
    }

    public event Action<int> OnRenownChanged;

    /// <summary>
    /// 세이브 데이터 로드
    /// </summary>
    /// <param name="index">세이브 번호</param>
    public void LoadSaveData(int index = 0)
    {
        _saveData = new SaveData();

        DataLoader.GetPlayerData(index);
    }

    /// <summary>
    /// 명성치 조건 확인
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool CheckRenown(int condition)
    {
        if (_saveData.renown >= condition)
        {
            return true;
        }

        return false;
    }
    
    
    private DailyData _dayData;
    #endregion

    #region DayManage
    //////// 날짜 및 시간 관리 //////// 

    private uint _dateIndex;
    private uint _timeIndex;

    public event Action<int> OnDateChanged;
    public event Action<int> OnTimeChanged;
    public uint GetDate()
    {
        return _dateIndex;
    }

    public uint GetTime()
    {
        return _timeIndex;
    }

    public Date GetDateInfo()
    {
        return _dayData.date;
    }

    public void SetDate(uint date)
    {
        DailyData newData = LoadDayData(date);
        if (newData == null) return;

        // 데이터 세이브
        SavePlayerData();
        
        _dateIndex = date;
        
        OnDateChanged?.Invoke((int)date);
    }

    public void SetTime(uint time)
    {
        if (time < 0 || time > 3) return;

        _timeIndex = time;
        
        OnTimeChanged?.Invoke((int)time);
    }

    /// <summary>
    /// 해당 날짜 데이터 로드
    /// </summary>
    /// <param name="index">로드할 날짜 인덱스</param>
    private DailyData LoadDayData(uint index)
    {
        return DataLoader.GetDayData(index);
    }

    /// <summary>
    /// 플레이어 데이터 저장
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void SavePlayerData()
    {
        DataLoader.PushPlayerData(_saveData, _dateIndex);
    }

    #endregion
    
    #region WorkManage
    //////// 업무 관리 ////////
    
    public bool isScreenOn { get; set; }
    
    public event Action OnWorkClear;
    
    public List<Work> GetList()
    {
        return _dayData.workList;
    }

    public int GetStage(string workCode)
    {
        Work work = _dayData.workList.Find(w => w.code == workCode);
        if (work == null)
        {
            return -1;
        }

        return work.stage;
    }

    public void ClearWork(string workCode)
    {
        Work work = _dayData.workList.Find(w => w.code == workCode);
        if (work == null)
        {
            return;
        }
        work.isClear = true;
        
        // 전체 클리어 여부 확인
        if (IsWorkClear())
        {
            OnWorkClear?.Invoke();
        }
    }

    public bool IsWorkClear()
    {
        foreach (var w in _dayData.workList)
        {
            if (!w.isClear)
            {
                return false;
            }
        }

        return true;
    }
    
    #endregion
}
