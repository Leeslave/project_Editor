using GameService;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
    
    private int[] _bgmCode = new int[(int)World.Max]; 

    [Header("지역 효과")]
    [SerializeField] private int nightShift;
    [SerializeField] private GameObject buttons;
    [SerializeField] private FadeCurtain curtain;      // 지역 이동 효과 이미지
    public SoundManager worldBGM;  // 지역 내 배경음악

    public void Awake()
    {
        for (int i = 0; i < _bgmCode.Length; i++)
        {
            _bgmCode[i] = i;
        }
    }

    public void Start()
    {
        // 서비스 구독
        GameSystem.GetService<ILocationService>().OnPosChanged += MoveLocation;
        GameSystem.GetService<IDayService>().OnTimeChanged += SetTime;
    }
    
    private void SetTime(int time, TimeData timeData)
    {
        bool isNight = time > 1;
        
        // 시간대 외형 설정
        transform.position = isNight ? 
            new Vector3(transform.position.x, nightShift, transform.position.z) 
            : new Vector3(transform.position.x, 0, transform.position.z);
        
        // Object 설정
        WorldObjectFactory.Instance.Init(timeData.npc, timeData.action);
        
        // BGM 설정
        InitBGM(timeData.bgm);
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
    private void MoveLocation(WorldVector vector)
    {
        if (vector == null) return;
        
        // 위치 이동
        int x = (int)vector.location * 1000 + vector.position * 100;
        mainCamera.transform.position = new Vector3(x, 0, 0);
    
        // BGM 설정
        worldBGM.SetClip(_bgmCode[(int)vector.location], true);
    }
    
    #endregion

    // BGM 코드 초기화
    private void InitBGM(List<BGMData> data = null)
    {
        // BGM 기본값 적용
        for (int i = 0; i < _bgmCode.Length; i++)
        {
            _bgmCode[i] = i;
        }

        if (data == null)
        {
            return;
        }

        // 변경사항 적용
        foreach (var iter in data)
        {
            _bgmCode[(int)iter.location] = iter.code;
        }
    }

    private void OnDestroy()
    {
        GameSystem.GetService<ILocationService>().OnPosChanged -= MoveLocation;
        GameSystem.GetService<IDayService>().OnTimeChanged -= SetTime;
    }
}