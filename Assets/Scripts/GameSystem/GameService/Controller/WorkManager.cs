
using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkManager : Singleton<WorkManager>
{
    /**
     * 업무 목록 관리 서비스
     * - 업무 데이터 관리
     * - 완료 처리
     */
    private IDayService _dayService;
    
    public bool isScreenOn { get; set; }
    
    public event Action OnWorkClear;
    [SerializeField] private List<Work> works = new();
    
    // TODO: WorkData SO 추가할 시 여기서 직접 전달
    // private List<WorkData> workData = new();

    private void Start()
    {
        _dayService = GameSystem.Instance.GetService<IDayService>();
        _dayService.OnDayChanged += Init;
    }
    
    /// <summary>
    /// 업무 목록 갱신 - 데이터에서 로드
    /// </summary>
    private void Init(DailyData data)
    {
        works = data.workList;
    }

    /// <summary>
    /// 전체 업무 리스트 반환
    /// </summary>
    /// <returns>업무 리스트</returns>
    public List<string> GetList()
    {
        return works.Select(x => x.name).ToList();
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
        var target = works.Find(work => work.code == workCode);
        return target?.stage ?? -1;
    }

    /// <summary>
    /// 업무 코드로 해당 업무 완료 처리
    /// </summary>
    /// <param name="workCode">완료할 업무 코드</param>
    /// <remarks>모든 업무 완료 시 이벤트 실행</remarks>
    public bool ClearWork(string workCode)
    {
        var target = works.Find(work => work.code == workCode);

        if (target != null) target.isClear = true;
        
        return IsWorkClear();
    }

    /// <summary>
    /// 모든 업무 완료 여부 확인
    /// </summary>
    /// <returns>업무 완료 여부</returns>
    public bool IsWorkClear()
    {
        bool isClear = works.All(work => work.isClear);
        if (isClear) OnWorkClear?.Invoke();
        return isClear;
    }

    private void OnDestroy()
    {
        _dayService.OnDayChanged -= Init;
    }
}
