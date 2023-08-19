using System;
using System.Collections;
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
    Hallway,
    Office
}

public class WorldSceneManager : MonoBehaviour 
{
    /**
    * MainWorld 씬 매니저
    *   - 날짜, 시간대 맞춰서 지역 동기화
    *   - 지역 내 위치 이동
    *   - 지역 간 이동
    */

    [SerializeField]
    private GameObject[] locationList;    // 지역 오브젝트 리스트
    [SerializeField]
    private GameObject introObject;      // 인트로 오브젝트
    [SerializeField]
    private List<(GameObject, NPCSchedule)> npcList;     // 모든 지역 NPC 리스트

    void Start()
    {
        // npc 리스트 초기화
        npcList = new();

        // 위치 동기화
        SetLocation((int)GameSystem.Instance.location);

        // 추가 인트로 실행 (날짜 변경 후 0시에만)
        if (GameSystem.Instance.time == 0)
        {
            if (introObject)
                introObject.SetActive(true);
        }
    }

    /// <summary>
    /// 세이브 지역와 활성화 월드 동기화
    /// </summary>
    /// <param name="location">이동할 지역의 이름 입력</param>
    /// <remarks>새 지역 오브젝트 활성화</remarks>
    public void SetLocation(int location)
    {
        // 이동할 지역 설정
        if (!Enum.IsDefined(typeof(World), location))
        {
            Debug.Log("Location number ERROR");
            location = 0;
        }

        // 해당하는 지역만 활성화
        for(int i = 0; i < locationList.Length; i++)
        {
            locationList[i].SetActive(false);
            if (i == location)
                locationList[i].SetActive(true);
        }

        // 현재 지역 설정
        GameSystem.Instance.location = (World)location;

        // 지역 내 위치 동기화
        SetPosition(GameSystem.Instance.position);
    }

    /// 위치 내 이동
    public void SetPosition(int position)
    {
        // 현재 월드 오브젝트
        Transform currentWorldObject = locationList[(int)GameSystem.Instance.location].transform;

        // 위치값 예외 처리
        if (position < 0 || GameSystem.Instance.position >= currentWorldObject.childCount)
            position = 0;
        
        // 해당 위치 활성화
        for(int i = 0; i < currentWorldObject.childCount; i++)
        {
            currentWorldObject.GetChild(i).gameObject.SetActive(false);
            if (i == position)
                currentWorldObject.GetChild(i).gameObject.SetActive(true);
        }

        // 현재 위치 설정
        GameSystem.Instance.position = position;
    }

    /// <summary>
    /// 시간대 변경
    /// </summary>
    /// <remarks>시간대를 변경하고 현재 지역 동기화
    public void ChangeTime(int time)
    {
        /**
        시간대 변경에 따른 지역들 동기화
        - NPC들 활성화, 위치 동기화
        - 지역들 배경 이미지 변경
        - 바로 다음 시간대로만 변경
        */

        // 시간대 변경 제한
        if (time < 1 || time > 4)
            return;
        
        // 해당하는 시간 변경이 아닐 시 종료
        if (time != GameSystem.Instance.time + 1)
        {
            return;
        }

        // 날짜 변경
        if (time == 4)
        {
            GameSystem.Instance.SetDate();
            return;
        }

        // 시간 변경
        GameSystem.Instance.SetTime(time);
    }
}