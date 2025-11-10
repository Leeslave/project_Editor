using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum World {
    Street,
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

    private List<int> _blockList = new();    // 지역 이동 제한 리스트
    [SerializeField] private int nightShift;
    [SerializeField] private GameObject buttons;
    
    [Header("지역 효과")]
    [SerializeField] private FadeCurtain curtain;      // 지역 이동 효과 이미지
    public SoundManager worldBGM;  // 지역 내 배경음악

    public void Init(List<int> blockList, World startLocation, int startPosition = 0, bool isNight = false)
    {
        if (isNight)
        {
            transform.position = new Vector3(transform.position.x, nightShift, transform.position.z);
        }
        _blockList = blockList;
    }

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
    /// <returns>좌표 기준으로 지역 이동</returns>
    public bool MoveLocation(int val)
    {
        // 블럭 확인
        if (_blockList.Contains(val)) return false;
        
        // 위치 이동
        mainCamera.transform.position = new Vector3(val, 0, 0);
        worldBGM.SetClip(val / 1000, true);
        return true;
    }
    
    /// <summary>
    /// 지역 변경
    /// </summary>
    /// <remarks>위치 기준으로 지역 이동</remarks>
    public bool MoveLocation(World location, int position)
    {
        int x = (int)location * 1000 + position * 100;
        return MoveLocation(x);
    }

}