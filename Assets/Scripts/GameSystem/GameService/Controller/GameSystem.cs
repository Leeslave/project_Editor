using System;
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
    
    public void Init()
    {
        // 서비스 주입
        ServiceProvider.Register<ISaveService>(this);
        
        // TODO: 세이브 파일 무결성 확인
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
    #endregion
}
