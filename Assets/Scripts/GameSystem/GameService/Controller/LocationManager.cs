using GameService;
using System;
using System.Collections.Generic;
using System.Linq;

public class LocationManager : Singleton<LocationManager>
{
    private IDayService _dayService;
    
    public List<WorldVector> BlockList { get; set; }    // 지역 이동 제한 리스트
    public event Action<WorldVector> OnPosChanged;

    private WorldVector _vector;
    public WorldVector currentVector 
    { 
        get => _vector;
        set => MoveLocation(value);
    }

    private void Start()
    {
        _dayService = GameSystem.Instance.GetService<IDayService>();

        _dayService.OnTimeChanged += TimeInit;
        _dayService.OnDayChanged += DayInit;
    }

    private void DayInit(DailyData data)
    {
        currentVector = data.startLocation;
    }

    private void TimeInit(TimeData timeData)
    {
        // 지역 제한 설정
        BlockList = timeData.block;
        
        // NOTE: LocationManager에서 BGM 관리할 시 지역별 BGM 로드
        
        // 월드오브젝트 생성
        WorldObjectFactory.Instance.Init(timeData.npc, timeData.action);
    }

    public bool MoveLocation(WorldVector vector)
    {
        // 블록 확인
        if (BlockList.Any(p => p == vector))
        {
            return false;
        }
        
        // 위치 이동
        _vector = vector;
        OnPosChanged?.Invoke(_vector);
        return true;
    }

    void OnDestroy()
    {
        _dayService.OnTimeChanged -= TimeInit;
        _dayService.OnDayChanged -= DayInit;
    }
}
