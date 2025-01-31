using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Location : MonoBehaviour
{

    [SerializeField]
    private World locationName; // 지역명

    private bool isActive;  // 현재 활성화 여부

    public int bgmNumber;   // BGM 번호

    [SerializeField]
    private int connectLen;     // 연결 가능 최대 길이

    [SerializeField] public List<Position> Positions;
    [SerializeField] public List<GameObject> buttons;   // 지역 내 위치 이동 버튼들

    private List<List<GameObject>> objList = new();  // WorldObject 리스트
    public float sizeMultiplier = 1;    // NPC 크기 배율


    /// <summary>
    /// 현재 지역을 활성화/비활성화
    /// </summary>
    public void ActiveLocation(bool _active)
    {
        // 변화 없음 예외
        if (isActive == _active)
            return;
        
        // 활성화
        if (_active)
        {
            // 음악 활성화
            SetBGM(bgmNumber);

            // 해당하는 위치 활성화
            SetPosition(GameSystem.Instance.currentLocation.position);
        }
        // 비활성화
        else
        {
            // 위치 전부 비활성화
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        isActive = _active;
    }


    /// <summary>
    /// 지역 재로딩, 날짜&시간대 재적용
    /// </summary>
    [HideInInspector]
    public void ReloadLocation()
    {
        // NPC 새로 생성
        SetObjects();

        // 기본으로 비활성화
        ActiveLocation(false);
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
            Debug.Log($"WORLD MOVE ERROR : Invalid position {locationName} - {newPos}");
            return;
        }

        // 페이드인아웃 효과 실행
        StartCoroutine(WorldSceneManager.Instance.FadeInOut());

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
        // 나머지 장소 비활성화
        for(int i = 0; i < Positions.Count; i++)
        {
            if (i == newPos)
            {
                continue;
            }
            Positions[i].gameObject.SetActive(false);
        }
        
        // 장소 데이터 변경
        // GameSystem.Instance.gameData.SetPosition(newPos);
    }


    public void SetBGM(int bgm)
    {
        // 음악 활성화
        WorldSceneManager.Instance.worldBGM.OverlapPlay(bgm);
    }


    // 연결된 맵 왼쪽으로 이동
    public void MoveLeft()
    {
        int newPos = GameSystem.Instance.currentLocation.position - 1;

        SetPosition(newPos);
    }


    // 연결된 맵 오른쪽으로 이동
    public void MoveRight()
    {
        int newPos = GameSystem.Instance.currentLocation.position + 1;

        SetPosition(newPos);
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
    /// 시간대에 해당하는 NPC들 생성
    /// </summary>
    /// <param name="date">날짜</param>
    /// <param name="time">시간대</param>
    public void SetObjects()
    {
        // // 오브젝트 리스트 초기화
        // ClearObjects();
        //
        // // 새 오브젝트 정보 불러오기
        // List<WorldObjectData> dataList = ObjectDatabase.ObjectList[(int)locationName];
        //
        // // NPC들 생성
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
