
using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationController : MonoBehaviour, ILocationService
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

    private void Awake()
    {
        ServiceProvider.Register<ILocationService>(this);
    }

    private void Start()
    {
        ServiceProvider.Get<IDayService>(service =>
        {
            _dayService = service;
            _dayService.OnTimeChanged += _ => BlockList = _dayService.TimeData.block;
            _dayService.OnDateChanged += _ => currentVector = _dayService.GetStartLocation();
            Init();
        });
        
    }
    
    public void Init()
    {
        BlockList = _dayService.TimeData.block;
        currentVector = _dayService.GetStartLocation();
    }

    public WorldVector MoveLocation(WorldVector vector = null)
    {
        if (vector == null)
        {
            currentVector = _dayService.GetStartLocation();
            return currentVector;
        }
        
        // 블록 확인
        if (BlockList.Any(p => p == vector))
        {
            return null;
        }
        
        // 위치 이동
        _vector = vector;
        OnPosChanged?.Invoke(_vector);
        return _vector;
    }
}
