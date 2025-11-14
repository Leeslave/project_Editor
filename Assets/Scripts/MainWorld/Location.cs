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
        // List<ChatObjectData> npcs = GameSystem.Instance.DayData 
        //                                         .dayTimes[GameSystem.Instance.timeIndex].npc
        //                                         .Where(npc=> npc is { positions: { Count: > 0 } } &&
        //                                                      npc.positions[0].location == locationName)
        //                                         .ToList();
        
        List<ChatObjectData> npcs = GetNpcsByLocation(locationName);
        foreach(ChatObjectData npc in npcs)
        {
            WorldObjectFactory.Instance.CreateNPC(npc, locationName, Positions[npc.positions[0].position].transform);
        }
    }
    
    public List<ChatObjectData> GetNpcsByLocation(World locationName)
    {
        // 1. GameSystem.Instance와 DayData 체크
        if (GameSystem.Instance == null)
        {
            Debug.LogError("DEBUG: GameSystem.Instance가 null입니다. 초기화되었는지 확인하세요.");
            return new List<ChatObjectData>();
        }

        var dayData = GameSystem.Instance.DayData;
        if (dayData == null)
        {
            Debug.LogError("DEBUG: GameSystem.Instance.DayData가 null입니다.");
            return new List<ChatObjectData>();
        }

        // 2. timeIndex 유효성 및 dayTimes 체크
        int timeIndex = GameSystem.Instance.timeIndex;
        var dayTimes = dayData.dayTimes;

        if (dayTimes == null)
        {
            Debug.LogError("DEBUG: DayData.dayTimes 리스트가 null입니다.");
            return new List<ChatObjectData>();
        }
        
        if (timeIndex < 0 || timeIndex >= dayTimes.Length)
        {
            Debug.LogError($"DEBUG: timeIndex({timeIndex})가 dayTimes 리스트의 범위를 벗어납니다. (Count: {dayTimes.Length})");
            return new List<ChatObjectData>();
        }

        var currentDayTime = dayTimes[timeIndex];
        if (currentDayTime == null)
        {
            Debug.LogError($"DEBUG: dayTimes[{timeIndex}] 요소가 null입니다.");
            for (int i = 0; i < dayTimes.Length; i++)
            {
                Debug.Log($"{i} = > {dayTimes[i]}");
            }
            return new List<ChatObjectData>();
        }

        // 3. NPC 리스트 체크
        var npcList = currentDayTime.npc;
        if (npcList == null)
        {
            Debug.LogWarning($"DEBUG: currentDayTime.npc 리스트가 null입니다. (NPC가 없는 것으로 간주하고 빈 리스트 반환)");
            return new List<ChatObjectData>();
        }

        // 4. LINQ 쿼리 (안전한 필터링 및 디버그)
        // List<ChatObjectData> 대신 var를 사용했습니다. (반환 타입이 ChatObjectData의 리스트임을 가정)
        var npcs = npcList
            .Where(npc => {
                // 개별 NPC 객체 자체 Null 체크
                if (npc == null)
                {
                    Debug.LogWarning("DEBUG: npc 리스트 안에 null인 요소가 있습니다. 스킵합니다.");
                    return false;
                }

                // positions 리스트 Null 체크
                if (npc.positions == null)
                {
                    Debug.LogWarning($"DEBUG: NPC '{npc.name}'의 positions 리스트가 null입니다. 스킵합니다.");
                    return false;
                }

                // positions Count 체크
                if (npc.positions.Count == 0)
                {
                    // Count가 0인 것은 원래 조건에 의해 제외되지만, 로그를 남깁니다.
                    Debug.LogWarning($"DEBUG: NPC '{npc.name}'의 positions 리스트가 비어있습니다. 스킵합니다.");
                    return false;
                }

                // positions[0] 요소 Null 체크
                if (npc.positions[0] == null)
                {
                    Debug.LogWarning($"DEBUG: NPC '{npc.name}'의 positions[0]가 null입니다. 스킵합니다.");
                    return false;
                }

                // 위치 일치 조건
                if (npc.positions[0].location == locationName)
                {
                    Debug.Log($"DEBUG: NPC '{npc.name}'가 '{locationName}'에서 발견되었습니다.");
                    return true;
                }
                
                return false;
            })
            .ToList();

        return npcs;
    }
}
