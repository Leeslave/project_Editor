using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[Flags]
public enum World {
    Street = 0,
    Bar,
    Cafe,
    Restaurant,
    Temple,
    Hallway,
    Office,
    Office3,
    Interrogate,
    
    Max
} 

public class WorldSceneManager : Singleton<WorldSceneManager>
{
    /**
    * MainWorld 씬 매니저
    *   - 지역 내 위치 이동
    *   - 지역 간 이동
    */
    public GameObject mainCamera;
    private IDayService dayService;
    
    private List<WorldVector> _blockList = new();    // 지역 이동 제한 리스트
    private Dictionary<World, int> _bgmCode = new(); 
    private bool isNight => (dayService.Time > 1);

    [Header("지역 효과")]
    [SerializeField] private int nightShift;
    [SerializeField] private GameObject buttons;
    [SerializeField] private FadeCurtain curtain;      // 지역 이동 효과 이미지
    public SoundManager worldBGM;  // 지역 내 배경음악

    public void Start()
    {
        dayService = ServiceProvider.Get<IDayService>(service =>
        {
            dayService = ServiceProvider.Get<IDayService>();
            dayService.OnDateChanged += _ => Init();
            dayService.OnTimeChanged += _ =>
            {
                GetTimeData();
                SetNight();
            };
        });
        
        // BGM Init
        InitBGM();
    }
    
    public void Init()
    {
        GetTimeData();
        SetNight();
        SetStartPos();
    }

    private void SetStartPos()
    {
        // 시작 지점 설정
        var start = dayService.GetStartLocation();
        MoveLocation(start.location, start.position);
    }

    private void GetTimeData()
    {
        // 지역 설정
        var timeData = dayService.TimeData;
        _blockList = timeData.block;
        // BGM 매칭
        InitBGM(timeData.bgm);
    }

    private void SetNight()
    {
        // 시간대 외형 설정
        if (isNight)
        {
            transform.position = new Vector3(transform.position.x, nightShift, transform.position.z);
        }
    }

    #region Move
    /// <summary>
    /// 상호작용 활성화/비활성화
    /// </summary>
    public void SwitchInteraction()
    {
        buttons.SetActive(!buttons.activeSelf);
    }
    
    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>위치 기준으로 지역 이동</remarks>
    public bool MoveLocation(World location, int position)
    {
        // 블록 확인
        if (_blockList.Any(p => p.location == location && p.position == position))
        {
            Debug.Log($"Move Blocked : {location} : {position}");
            return false;
        }
        
        // 위치 이동
        int x = (int)location * 1000 + position * 100;
        mainCamera.transform.position = new Vector3(x, 0, 0);
        
        // BGM 설정
        worldBGM.SetClip(_bgmCode[location], true);
        return true;
    }
    
    #endregion

    // BGM 코드 초기화
    private void InitBGM(List<BGMData> data = null)
    {
        // BGM 기본값 적용
        foreach (World code in Enum.GetValues(typeof(World)))
        {
            _bgmCode[code] = (int)code;
        }
        if (data == null)
        {
            return;
        }
        
        // 변경사항 적용
        foreach (var iter in data)
        {
            _bgmCode[iter.location] = iter.code;
        }
    }
}