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

    public string worldPath = "Prefab/MainWorld/";  // 월드 프리팹 경로
    [SerializeField]
    [System.Serializable]
    public enum World {
        Cafe,
        Street,
        Office
    }
    public World currentWorld;

    private GameObject currentWorldObject = null;    // 현재 월드 오브젝트
    public GameObject introObject;      // 인트로 오브젝트

    void Start()
    {
        asyncLocation(0);
        asyncDate();
    }

    /// <summary>
    /// 세이브 위치와 활성화 월드 동기화
    /// </summary>
    /// <param name="WorldName">이동할 월드의 이름 입력</param>
    /// <remarks>씬 내 WorldCanvas 객체 삭제 후 새로 생성</remarks>
    public void asyncLocation(int world)
    {
        // 이동할 위치 설정
        if (!Enum.IsDefined(typeof(World), world))
        {
            world = 0;
        }

        // 새 월드
        GameObject newWorld = transform.GetChild(world).gameObject;
        if (currentWorldObject == newWorld) // 동일월드로 재이동
            return;

        // 새 월드 적용 및 위치 설정
        currentWorldObject.SetActive(false);
        newWorld.SetActive(true);
        currentWorldObject = newWorld;
    }

    /// <summary>
    /// 시간대, 날짜 동기화
    /// </summary>
    /// <remarks>현재 날짜, 시간대에 맞춰 씬 동기화 및 새로고침</remarks>
    public void asyncDate()
    {
        if (!currentWorldObject)
            return;
        
        // 월드들 시간대 동기화
        

        // 추가 인트로 실행 (날짜 변경 후 0시에만)
        if (GameSystem.Instance.player.time == 0)
        {
            if (introObject)
                introObject.SetActive(true);
        }
    }


}