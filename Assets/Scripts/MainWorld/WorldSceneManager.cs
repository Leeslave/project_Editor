using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSceneManager : MonoBehaviour 
{
    /**
    * MainWorld 씬 매니저
    *   시간대 변경
    *   - 날짜, 시간대 맞춰서 월드캔버스 동기화
    *   WorldCanvas 관리
    *   - 월드 캔버스 현재 위치 동기화
    *   - 월드캔버스간 이동
    *   월드 자체 활성화/비활성화
    */

    public string worldPath = "Prefab/MainWorld/";  // 월드 프리팹 경로
    public WorldCanvas currentWorld = null;    // 현재 월드 오브젝트
    public GameObject introObject;

    void Start()
    {
        asyncLocation();
        asyncDate();
    }

    /// <summary>
    /// 시간대, 날짜 동기화
    /// </summary>
    /// <remarks>현재 날짜, 시간대에 맞춰 씬 동기화 및 새로고침</remarks>
    public void asyncDate()
    {
        // 현재 월드 시간대 동기화
        if (currentWorld)
            currentWorld.asyncWorldTime();
        // 추가 인트로 실행 (날짜 변경 후 0시에만)
        if (GameSystem.Instance.player.time == 0)
        {
            if (introObject)
                introObject.SetActive(true);
        }
    }

    /// <summary>
    /// 세이브 위치와 활성화 월드 동기화
    /// </summary>
    /// <param name="WorldName">이동할 월드캔버스의 이름 입력</param>
    /// <remarks>씬 내 WorldCanvas 객체 삭제 후 새로 생성</remarks>
    public void asyncLocation(string worldName = "")
    {
        // 이동할 위치 설정
        if (worldName != "")
        {
            GameSystem.Instance.player.location = worldName;
        }

        // 현재 월드 삭제
        if (currentWorld != null)
        {
            Destroy(currentWorld.gameObject);
        }
        // 새 월드 생성
        GameObject newWorld = Resources.Load<GameObject>(worldPath + GameSystem.Instance.player.location + "Canvas");
        if (newWorld == null) newWorld = Resources.Load<GameObject>(worldPath + "OfficeCanvas");    // 새 월드 오류시 Office 기본값
        // 새 월드 적용
        Debug.Log($"New WorldCanvas Loaded : {newWorld.name}");
        GameObject newWorldObject = Instantiate(newWorld);
        newWorldObject.transform.SetParent(transform);
        currentWorld = newWorldObject.GetComponent<WorldCanvas>();
    }
}