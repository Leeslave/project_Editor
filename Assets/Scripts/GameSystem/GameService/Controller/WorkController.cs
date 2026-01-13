
using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkController : MonoBehaviour, IWorkService
{
    /**
     * 업무 목록 관리 서비스
     * - dataService 기반
     * - 업무 완료 처리
     */
    
    public bool isScreenOn { get; set; }
    public event Action OnWorkClear;
    
    [SerializeField] private List<Work> works = new();
    private IDataService _dataService;

    private void Awake()
    {
        ServiceProvider.Register<IWorkService>(this);
    }

    private void Start()
    {
        _dataService = ServiceProvider.Get<IDataService>();
        _dataService.OnDataChanged += _ => Init();      // 데이터 변경 시 자동 갱신
    }
    
    /// <summary>
    /// 업무 목록 갱신 - 데이터에서 로드
    /// </summary>
    public void Init()
    {
        works = _dataService.GetWorkList();
    }

    /// <summary>
    /// 전체 업무 리스트 반환
    /// </summary>
    /// <returns>업무 리스트</returns>
    public List<Work> GetList()
    {
        return works;
    }

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
    public void ClearWork(string workCode)
    {
        var target = works.Find(work => work.code == workCode);

        if (target != null) target.isClear = true;
        
        if (IsWorkClear()) OnWorkClear?.Invoke();
    }

    /// <summary>
    /// 모든 업무 완료 여부 확인
    /// </summary>
    /// <returns>업무 완료 여부</returns>
    public bool IsWorkClear()
    {
        return works.All(work => work.isClear);
    }
}
