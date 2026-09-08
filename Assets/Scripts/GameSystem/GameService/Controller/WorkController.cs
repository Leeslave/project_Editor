
using GameAction;
using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

public class WorkController : ServiceBase<IWorkService>, IWorkService
{
    /**
     * 업무 목록 관리 서비스
     * - 업무 데이터 관리
     * - 완료 처리
     */
    public bool isScreenOn { get; set; }
    public string CurrentWorkCode { get; private set; }
    public event Action OnWorkClear;

    private readonly Dictionary<Work, bool> _workList = new();
    
    // TODO: WorkData SO 추가할 시 여기서 직접 전달
    // private List<WorkData> workData = new();
    
    /// <summary>
    /// 업무 목록 갱신 - 데이터에서 로드
    /// </summary>
    public void Init(DailyData data)
    {
        _workList.Clear();
        CurrentWorkCode = null;
        foreach (var work in data.workList)
        {
            _workList.Add(work, false);
        }
    }

    /// <summary>
    /// 전체 업무 리스트 반환
    /// </summary>
    /// <returns>업무 리스트</returns>
    public List<(string code, string name)> GetList()
    {
        return _workList.Keys.Select(work => (
            work.code,
            string.IsNullOrWhiteSpace(work.name) ? work.code : work.name.Trim())).ToList();
    }

    /// <summary>
    /// 업무 실행 정보 설정 및 로드할 씬 확인
    /// </summary>
    /// <param name="workCode">실행할 업무 코드</param>
    /// <param name="sceneName">실제로 로드할 씬 이름</param>
    /// <returns>미완료 업무이며 씬을 로드할 수 있는지 여부</returns>
    public bool TryStartWork(string workCode, out string sceneName)
    {
        sceneName = null;
        Work target = _workList.Keys.FirstOrDefault(work => work.code == workCode);
        if (target == null || _workList[target]) return false;

        string targetScene = workCode == "SecureDocument" ? "Document" : workCode;
        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            EditorLogger.LogWarning($"Work scene not available: {targetScene}");
            return false;
        }

        CurrentWorkCode = workCode;
        sceneName = targetScene;
        return true;
    }

    /// <summary>
    /// 개별 업무 완료 여부 확인 (미등록 업무는 false)
    /// </summary>
    /// <param name="workCode">확인할 업무 코드</param>
    public bool IsWorkClear(string workCode)
    {
        Work target = _workList.Keys.FirstOrDefault(work => work.code == workCode);
        return target != null && _workList[target];
    }

    // public WorkData GetWorkData(string code, int stage)
    // {
    //     
    // }

    /// <summary>
    /// 업무 코드로 해당 업무 스테이지 반환
    /// </summary>
    /// <param name="workCode">확인할 업무 코드</param>
    /// <returns>해당 업무 스테이지값 (없을 시 -1)</returns>
    public int GetStage(string workCode)
    {
        Work target = _workList.FirstOrDefault(
                        work => work.Key.code == workCode)
                        .Key;
        return target?.stage ?? -1;
    }

    /// <summary>
    /// 업무 코드로 해당 업무 완료 처리
    /// </summary>
    /// <param name="workCode">완료할 업무 코드</param>
    /// <returns>전체 업무 완료 여부 (미등록 코드는 false)</returns>
    /// <remarks>최초 전체 완료 시에만 시간 전환과 이벤트 발행</remarks>
    public bool ClearWork(string workCode)
    {
        Work target = _workList.FirstOrDefault(
                work => work.Key.code == workCode)
            .Key;
        if (target == null)
        {
            EditorLogger.LogWarning($"Work not registered: {workCode}");
            return false;
        }
        if (_workList[target]) return IsWorkClear();
        _workList[target] = true;
        
        // 모든 업무 완료 확인
        bool onClear = IsWorkClear();
        if (onClear)
        {
            new NextTimeAction(2).Invoke();
            OnWorkClear?.Invoke();
        }
        
        return onClear;
    }

    /// <summary>
    /// 모든 업무 완료 여부 확인
    /// </summary>
    /// <returns>업무 완료 여부</returns>
    public bool IsWorkClear()
    {
        // 업무가 없는 날도 조회만 수행하며 시간 진행은 스토리 액션에 맡김
        return _workList.Values.All(done => done);
    }
}
