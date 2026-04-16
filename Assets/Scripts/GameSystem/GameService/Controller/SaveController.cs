
using GameService;
using System;
using UnityEngine;
using Utility;

public class SaveController : MonoBehaviour, ISaveService
{
    public int dayID;
    private PlayerData _playerData;
    private DaySave _currentSave;
    
    public DaySave Save => _currentSave;

   protected void Awake()
    {
        GameSystem.Instance.RegisterService(this);
        
        // 세이브 데이터 로드
        _playerData = DataLoader.GetData<PlayerData>(DataLoader.Savepath);
        if (_playerData == null)
        {
            EditorLogger.Log("New Save Created!");
            _playerData = new PlayerData();
        }
    }
   
   /// <summary>
   /// 날짜 전환
   /// </summary>
   /// <param name="day">전환될 날짜 (분기 유지)</param>
   /// <remarks>세이브 파일 강제 리로드</remarks>
    public void SelectDay(int day)
    {
        int newID = day * 100 + dayID % 100;
        Reload();
    }

   /// <summary>
   /// 분기 전환
   /// </summary>
   /// <param name="branch">전환될 분기</param>
    public void SelectBranch(int branch)
    {
        // 현재 세이브 그대로 새 분기로 파생 (날짜 전환 전까지 저장 안됨)
        dayID = (dayID / 100) * 100 + branch;
        _currentSave.dayID = dayID;
    }
   
    /// <summary>
    /// 세이브 새로 로드
    /// </summary>
    private void Reload()
    {
        if (_playerData == null)
        {
            return;
        }
        
        // 기존세이브 저장
        _playerData.Save(_currentSave);
        
        // 세이브 로드
        _currentSave = _playerData[dayID];
        if (_currentSave == null)
        {
            EditorLogger.Log("New Day Created!");
            _currentSave = new DaySave();
        }
    }

    public int Renown
    {
        get => _currentSave.renown;
        set
        {
            _currentSave.renown = value;
            OnRenownChanged?.Invoke(value);
        }
    }

    public event Action<int> OnRenownChanged;

    public bool CheckRenown(int condition)
    {
        return Renown >= condition;
    }

    private void OnDestroy()
    {
        GameSystem.Instance.UnRegister(this);
    }
}
