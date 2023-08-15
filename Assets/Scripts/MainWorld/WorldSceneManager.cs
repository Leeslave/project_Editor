using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSceneManager : MonoBehaviour 
{
    /**
    * MainWorld 씬 매니저
    *   시간대 변경
    *   - 날짜, 시간대 맞춰서 월드 동기화
    *   - 월드 내 위치 이동
    *   - 월드 간 이동
    */
    [SerializeField]
    public World currentWorld;  // 현재 월드

    [SerializeField]
    private int currentPosition;    // 현재 월드 내 위치
    [SerializeField]
    private GameObject[] worldList; // 월드 오브젝트 리스트
    [SerializeField]
    private GameObject introObject;      // 인트로 오브젝트

    void Start()
    {
        // 위치 동기화
        SetLocation((int)GameSystem.Instance.player.location);
        // 시간대 동기화
        SetDate(GameSystem.Instance.player.time);
    }

    /// <summary>
    /// 세이브 위치와 활성화 월드 동기화
    /// </summary>
    /// <param name="WorldName">이동할 월드의 이름 입력</param>
    /// <remarks>새 월드 오브젝트 활성화</remarks>
    public void SetLocation(int world)
    {
        // 이동할 위치 설정
        if (!Enum.IsDefined(typeof(World), world))
        {
            world = 0;
        }

        // 해당하는 월드만 활성화
        for(int i = 0; i < worldList.Length; i++)
        {
            worldList[i].SetActive(false);
            if (i == world)
                worldList[i].SetActive(true);
        }
        currentWorld = (World)world;
    }

    /// <summary>
    /// 시간대, 날짜 동기화
    /// </summary>
    /// <remarks>현재 날짜, 시간대에 맞춰 씬 동기화 및 새로고침</remarks>
    public void SetDate(int time)
    {
        if (time < 0 || time > 3)
            time = GameSystem.Instance.player.time;
        // TODO: 월드들 시간대 동기화
        
        GameSystem.Instance.ChangeTime(time);

        // 추가 인트로 실행 (날짜 변경 후 0시에만)
        if (time == 0)
        {
            if (introObject)
                introObject.SetActive(true);
        }
    }

    /// 위치 내 이동
    public void MovePosition(int position)
    {
        // 현재 월드 오브젝트
        Transform currentWorldObject = worldList[(int)currentWorld].transform;

        // 월드 내 포지션 설정
        if (position < 0 || currentPosition >= currentWorldObject.childCount)
            position = 0;
        
        // 해당 포지션 활성화
        for(int i = 0; i < currentWorldObject.childCount; i++)
        {
            currentWorldObject.GetChild(i).gameObject.SetActive(false);
            if (i == position)
                currentWorldObject.GetChild(i).gameObject.SetActive(true);
        }
    }
}