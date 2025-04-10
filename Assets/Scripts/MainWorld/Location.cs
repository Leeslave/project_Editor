using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public enum World {
    /**
    월드 내 지역 목록
    default : Street
    */
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

public class Location : MonoBehaviour
{
    /**
     지역 데이터
     - 오브젝트 배치
     - 지역 활성화/비활성화
     */
    [SerializeField]
    private World locationName; // 지역명

    public int bgmCode;   // BGM 번호

    [SerializeField] public List<Position> Positions;   // 지역 내 위치
    [SerializeField] public List<Door> buttons;   // 지역 내 위치 이동 버튼들


    /// <summary>
    /// 현재 지역을 활성화
    /// </summary>
    public void ActiveLocation(int position)
    {
        // 음악 활성화
        WorldSceneManager.Instance.worldBGM.OverlapPlay(bgmCode);

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


    /// <summary>
    /// 지역 이동 버튼 등록
    /// </summary>
    public void SetButtons()
    {
        // 버튼 오브젝트 불러오기
        buttons = GetComponentsInChildren<Door>(true).ToList();
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
    /// 해당하는 날짜 기준 BGM 코드 설정
    /// </summary>
    public void SetBGMCode()
    {
        BGMData bgmData = GameSystem.Instance.DayData
            .dayTimes[GameSystem.Instance.timeIndex].bgm
            .FirstOrDefault(bgm => bgm.location == locationName);

        if (bgmData is not null)
        {
            bgmCode = bgmData.code;
        }
    }

    
    /// <summary>
    /// 해당하는 날짜, 시간대에 월드 객체들 생성
    /// </summary>
    /// <param name="time">해당하는 시간대 (날짜는 해당 날짜 고정)</param>
    public void SetObjects()
    {
        // NPC 오브젝트 설정
        List<ChatObjectData> npcs = GameSystem.Instance.DayData 
                                                .dayTimes[GameSystem.Instance.timeIndex].npc
                                                .Where(npc=> npc.positions[0].location == locationName)
                                                .ToList();
        foreach(ChatObjectData npc in npcs)
        {
            WorldObjectFactory.Instance.CreateNPC(npc, locationName, Positions[npc.positions[0].position].transform);
        }
    }
}
