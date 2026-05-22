
using GameService;
using System;
using System.Collections.Generic;
using System.Linq;

public class WorkController : ServiceBase<IWorkService>, IWorkService
{
    /**
     * 업무 목록 관리 서비스
     * - 업무 데이터 관리
     * - 완료 처리
     */
    private IDayService _dayService;
    
    public bool isScreenOn { get; set; }
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
        foreach (var work in data.workList)
        {
            _workList.Add(work, false);
        }
    }

    /// <summary>
    /// 전체 업무 리스트 반환
    /// </summary>
    /// <returns>업무 리스트</returns>
    public List<string> GetList()
    {
        return _workList.Select(x => x.Key.name).ToList();
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
    /// <remarks>모든 업무 완료 검사</remarks>
    public bool ClearWork(string workCode)
    {
        Work target = _workList.FirstOrDefault(
                work => work.Key.code == workCode)
            .Key;
        if (target != null) _workList[target] = true;
        
        // 모든 업무 완료 확인
        bool onClear = IsWorkClear();
        if (onClear)
        {
            GameSystem.GetService<IDayService>().Time = 2;
        }
        
        return onClear;
    }

    /// <summary>
    /// 모든 업무 완료 여부 확인
    /// </summary>
    /// <returns>업무 완료 여부</returns>
    public bool IsWorkClear()
    {
        bool isClear = _workList.Values.All(done => true);
        if (isClear)
        {
            OnWorkClear?.Invoke();    
        }
        
        return isClear;
    }
}
