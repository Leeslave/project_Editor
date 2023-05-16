using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSceneManager : MonoBehaviour 
{
    /**
    * MainWorld 씬 매니저
    *   - 시간대 변경
    *   - 날짜, 시간대 맞춰서 월드캔버스 동기화
    *   - WorldCanvas 관리
    *   - 월드캔버스간 이동
    */

    public string worldPath = "Prefab/MainWorld/";  // 월드 프리팹 경로
    public WorldCanvas currentWorld = null;    // 현재 월드 오브젝트

    /// <summary>
    /// 세이브 위치와 활성화 월드 동기화
    /// </summary>
    /// <remarks>씬 내 WorldCanvas 객체 삭제 후 새로 생성</remarks>
    public void asyncWorldCanvas()
    {
        currentWorld = GameObject.FindObjectOfType<WorldCanvas>();
        if (currentWorld != null)
        {
            Destroy(currentWorld.gameObject);
        }
        GameObject newWorldObject = Instantiate(Resources.Load<GameObject>(worldPath + PlayerDataManager.Instance.saveData.location + "Canvas"));
        newWorldObject.transform.SetParent(transform);
        currentWorld = newWorldObject.GetComponent<WorldCanvas>();
    }

    /// <summary>
    /// 플레이어의 위치와 활성화 월드 동기화
    /// </summary>
    /// <param name="WorldName">이동할 월드캔버스의 이름 입력</param>
    /// <remarks>플레이어 데이터 설정 후 씬 내 WorldCanvas 객체 삭제 및 새로 생성</remarks>
    public void asyncWorldCanvas(string worldName)
    {
        PlayerDataManager.Instance.saveData.location = worldName;
        
        asyncWorldCanvas();
    }
}