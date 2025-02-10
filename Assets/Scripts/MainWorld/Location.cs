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
    
    NullMax
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

    private List<List<GameObject>> objList = new();  // WorldObject 리스트


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
        // TODO: Position의 비활성화 함수로 변경
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

        
        // TODO: Position 활성화 함수로 변경(오브젝트 활성화까지)
        // 이동할 장소 활성화
        Positions[newPos].gameObject.SetActive(true);
        // 이동할 장소의 오브젝트 활성화
        foreach(var obj in objList[newPos])
        {
            // if (obj.TryGetComponent(out WorldObject npc))
            // {
            //     npc.OnActive();
            // }
            // else 
            // {
            //     obj.TryGetComponent(out WorldEffect effect);
            //     effect.OnActive();
            // }
        }
        
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
        // 오브젝트 리스트 초기화
        ClearObjects();

        // // 새 오브젝트 정보 불러오기
        List<WorldObjectData> npcs = GameSystem.Instance.DayData.dayTimes[GameSystem.Instance.timeIndex].npcList;
        
        // // NPC들 생성
        foreach(WorldObjectData npc in npcs)
        {
            
        }
        // foreach(WorldObjectData _data in dataList)
        // {
        //     if (_data.time != GameSystem.Instance.gameData.time)
        //     {
        //         continue;
        //     }
        //     
        //     GameObject newObj = Instantiate(ObjectDatabase.Instance.prefabs[(int)_data.objType], Positions[_data.position].transform);     // instantiate 
        //     newObj.name = _data.name;
        //
        //     // 타입에 따라 컴포넌트 추가
        //     if (_data is EffectData)
        //     {
        //         var targetObject = newObj.GetComponent<WorldEffect>();
        //         targetObject.location = this;
        //         targetObject.data = _data as EffectData;
        //     }
        //     if(_data is ObjData)
        //     {
        //         var targetObject = newObj.GetComponent<WorldObject>();
        //         targetObject.location = this;
        //         targetObject.data = _data as ObjData;
        //         targetObject.SetPosition();
        //     }
        //     
        //     objList[_data.position].Add(newObj);
        // }
    }


    /// <summary>
    /// 특정 오브젝트 삭제
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    public void RemoveObject(int position, string name)
    {
        var obj = objList[position].Find(data => data.name == name);
        if (obj != null)
        {
            objList[position].Remove(obj);
            Destroy(obj);
        }
    }


    public void ClearObjects()
    {
        foreach(var iter in objList)
        {
            foreach(var obj in iter)
            {
                Destroy(obj);
            }
        }

        objList = new List<List<GameObject>>(transform.childCount);
        for (int i = 0; i < objList.Capacity; i++)
        {
            objList.Add(new());
        }
    }
}
