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

    public static string worldfilePath = "Prefab/MainWorld/";  // 월드 프리팹 경로
    /// <summary>
    /// 플레이어의 위치와 활성화 월드 동기화
    /// </summary>
    /// <remarks>씬 내 WorldCanvas 객체 삭제 후 새로 생성</remarks>
    public static void asyncWorldCanvas()
    {

        var existWorld = GameObject.FindObjectOfType<WorldCanvas>();
        if (existWorld != null)
        {
            Destroy(existWorld.gameObject);
        }
        // GameObject newWorldCanvas = Instantiate(Resources.Load<GameObject>(worldfilePath + playerData.location + "Canvas"));
    }
    //////////////////////////////////////////////////////////////////
}