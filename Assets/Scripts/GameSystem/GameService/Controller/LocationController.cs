using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationController : ServiceBase<ILocationService>, ILocationService
{
    private static int Time => GameSystem.GetService<IDayService>().Time;
    
    private List<WorldVector>[] BlockList = new List<WorldVector>[4];    // 지역 이동 제한 리스트
    public event Action<WorldVector> OnPosChanged;

    [SerializeField]
    private WorldVector _position;
    
    public WorldVector CurrentPosition => _position;

    public override void OnAwake()
    {
        for (int i = 0; i < 4; i++)
        {
            BlockList[i] = new List<WorldVector>();
        }
    }

    public void Init(DailyData data)
    {
        for (int i = 0; i < 4; i++)
        {
            BlockList[i] = data.dayTimes[i].block;
        }
        
        MoveLocation(data.startLocation);
    }

    public void MoveLocation(WorldVector vector)
    {
        // 블록 확인
        if (IsBlocked(vector))
        {
            return;
        }
        
        // 위치 이동
        _position = vector;
        OnPosChanged?.Invoke(_position);
    }

    public bool IsBlocked(WorldVector vector)
    {
        return BlockList[Time].Any(p => p == vector);
    }
}
