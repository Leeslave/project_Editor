using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum World {
    /**
    월드 내 지역 목록
    */
    Street,
    Bar,
    Cafe,
    Restaurant,
    Temple,
    Hallway,
    Office,
    Office2,
    Interrogate,
}

public class Location : MonoBehaviour
{
    /**
     지역 데이터
     - 오브젝트 배치
     - 지역 활성화/비활성화
     */
    [SerializeField]
    private World locationName; // 지역명

    public int bgmNumber;   // BGM 번호

    [SerializeField] public List<Position> Positions;   // 지역 내 위치
    [SerializeField] public List<GameObject> buttons;   // 지역 내 위치 이동 버튼들

    /// <summary>
    /// 현재 지역을 활성화
    /// </summary>
    public void ActiveLocation(int position)
    {
        // 음악 활성화
        SetBGM(bgmNumber);

        // 해당하는 위치 활성화
        SetPosition(position);
    }

    
    /// <summary>
    /// 현재 지역 비활성화
    /// </summary>
    public void InActiveLocation()
    {
        // 위치 전부 비활성화 
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    

    /// <summary>
    /// 지역 내 이동
    /// </summary>
    /// <param name="newPos">이동할 새 위치</param>
    public void SetPosition(int newPos)
    {
        // 위치값 오류
        if (newPos < 0 || newPos >= Positions.Count)
        {
            #if DEBUG
            Debug.LogWarning($"WORLD MOVE ERROR : Invalid position {locationName} - {newPos}");
            #endif
            
            return;
        }
        
        // 이동할 장소 활성화
        Positions[newPos].gameObject.SetActive(true);
        
        // TODO: Position 비활성화 함수로 변경(오브젝트 활성화까지)
        // 나머지 장소 비활성화
        for(int i = 0; i < Positions.Count; i++)
        {
            if (i == newPos)
            {
                continue;
            }
            Positions[i].gameObject.SetActive(false);
        }
    }


    // TODO: 각 지역별로 별개의 SoundSystem 소유 후 특수 클립만 가져다가 사용하도록 변경
    public void SetBGM(int bgm)
    {
        // 음악 활성화
        WorldSceneManager.Instance.worldBGM.OverlapPlay(bgm);
    }


    /// <summary>
    /// 지역 이동 버튼 활성화
    /// </summary>
    /// <param name="isActive">버튼 활성화 여부</param>
    [HideInInspector]
    public void SetButtonActive(bool isActive)
    {
        foreach(var button in buttons)
        {
            if (button != null)
                button.SetActive(isActive);
        }
    }


    /// <summary>
    /// 해당하는 날짜, 시간대에 월드 객체들 생성
    /// </summary>
    /// <param name="time">해당하는 시간대 (날짜는 해당 날짜 고정)</param>
    public void SetObjects()
    {
        // 새 오브젝트 정보 불러오기
        List<WorldObjectData> npcs = GameSystem.Instance.DayData.dayTimes[GameSystem.Instance.timeIndex].npc;
        
        // NPC들 생성
        foreach(WorldObjectData npc in npcs)
        {
            WorldObjectFactory.Instance.CreateObject(npc, locationName, Positions[npc.positions[0].position].transform);
        }
        
        // 지역 Block 설정
        // BGM 변경 설정
    }
}
