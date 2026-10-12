using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

public class SaveController : SaveService
{
    /**
     * 전체 플레이어 세이브 및 현재 날짜/분기 정보 관리
     * - 플레이어 데이터 목록
     * - 현재 선택 데이터 정보
     * - 날짜/분기 전환 트리거
     */
    
    // 저장 데이터
    private PlayerData _playerData;
    private DailyData _dailyData;
    
    /// <summary>
    /// 전체 세이브 데이터 로드 및 초기화
    /// </summary>
    public override void Init()
    {
        // 세이브 데이터 로드
        _playerData = DataLoader.GetPlayerData();
        if (_playerData == null)
        {
            // 새로 시작
            EditorLogger.Log("New Save Created!");
            _playerData = new PlayerData();
            _playerData.Init();
        }
        
        // 세이브 선택 초기화
        Save = null;
    }

    public override PlayerData GetPlayerData()
    {
        return _playerData;
    }

    /// <summary>
   /// 날짜 선택
   /// </summary>
   /// <param name="day">선택 날짜</param>
   /// <param name="branch">선택 분기</param>
   /// <remarks>세이브 파일 강제 리로드</remarks>
    public override void SelectDay(int day, int branch)
    {
        int newID = day * 100 + branch % 100;
        
        // 날짜 새로 선택
        if (_playerData[newID] == null)
        {
            EditorLogger.LogWarning("No Data Exists");
            return;
        }
        Save = _playerData[newID];

        NormalizeSave();

        Refresh();
    }

   /// <summary>
   /// 날짜 전환
   /// </summary>
    public override void SwitchDay()
    {
        if (Save == null)
        {
            EditorLogger.LogWarning("No Data Exists");
            return;
        }
        
        int newID = Save.dayID + 100;
        DaySave newSave = Save.Clone(newID);
        _playerData.Save(newSave);
        Save = newSave;
        Refresh();
    }

    /// <summary>
   /// 분기 전환
   /// </summary>
   /// <param name="branch">전환될 분기</param>
    public override void SwitchBranch(int branch)
    {
        if (Save == null)
        {
            EditorLogger.LogWarning("No Data Exists");
            return;
        }
        
        // 현재 세이브 그대로 새 분기로 파생
        int newID = DayIndex * 100 + branch;
        DaySave newSave = Save.Clone(newID);
        _playerData.Save(newSave);
        Save = newSave;
        Persist();
    }
   
    /// <summary>
    /// 세이브 새로 로드
    /// </summary>
    /// <remarks>날짜 변경 트리거</remarks>
    private void Refresh()
    {
        DataLoader.SavePlayerData(_playerData);
        
        // 날짜 데이터 로드
        _dailyData = DataLoader.GetDayData(DayIndex);
        
        // 서비스별 Init 실행 
        foreach (var service in GameSystem.GetAllServices())
        {
            service.Init(_dailyData);
        }
    }

    #region Renown

    public override bool CheckRenown(int condition)
    {
        return Renown >= condition;
    }

    public override bool IsWorkCompleted(string workCode, int stage = -1)
    {
        NormalizeSave();
        return Save != null && Save.completedWorks.Any(item => item.workCode == workCode &&
            (stage < 0 || item.stage == stage));
    }

    public override void RecordWorkCompletion(string workCode, int stage)
    {
        NormalizeSave();
        if (Save == null || IsWorkCompleted(workCode, stage)) return;
        Save.completedWorks.Add(new WorkCompletionRecord(workCode, stage));
        Persist();
    }

    public override bool TryApplyRenownSettlement(string settlementId, int delta)
    {
        NormalizeSave();
        if (Save == null || string.IsNullOrWhiteSpace(settlementId) ||
            Save.appliedSettlementIds.Contains(settlementId)) return false;
        Save.appliedSettlementIds.Add(settlementId);
        Renown += delta;
        Persist();
        return true;
    }

    private void NormalizeSave()
    {
        if (Save == null) return;
        Save.completedWorks ??= new List<WorkCompletionRecord>();
        Save.appliedSettlementIds ??= new List<string>();
    }

    private void Persist()
    {
        _playerData.Save(Save);
        DataLoader.SavePlayerData(_playerData);
    }
    
    #endregion
}
