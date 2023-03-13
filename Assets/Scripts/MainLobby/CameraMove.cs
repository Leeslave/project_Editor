using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    /**
    *   플레이어 이동하는 카메라 이동 스크립트
    *   - 현재 위치의 배경씬 활성화
    *   - 좌우로 카메라 이동
    *   - 스크린, 컷신 이후 최근 카메라 위치로 돌아오기
    */
    public List<GameObject> sceneList = new List<GameObject>();
    private int currentPosition;

    private void Start() {
        currentPosition = 0;
        ActiveCurrentScene();
    }

    private void ActiveCurrentScene()
    {
        foreach(var idx in sceneList)
            idx.SetActive(false);
        sceneList[currentPosition].SetActive(true);
    }

    // TODO
    /// <summary>
    /// 스크린 위치로 카메라 이동
    /// save 좌표로 이전 좌표 기억
    /// </summary>
    public void MoveToScreen()
    {

    }

    // TODO:
    /// <summary>
    /// 스크린 종료 시 최근 위치로 돌아가기
    /// </summary>
    public void MoveToMain()
    {
        ActiveCurrentScene();
    }

    /// <summary>
    /// 카메라 위치 옮기기
    /// - 좌우 : 옆 화면으로 이동하기(리스트로 관리)
    /// - 아래 : 이전 화면으로 이동 (축소 ,,,)
    /// - 위 : 미정
    /// </summary> 
    public void MoveCamera(string direction)
    {
        if (direction == "UP")
        {
            
        }
        if (direction == "DOWN")
        {
            
        }
        if (direction == "LEFT")
        {
            currentPosition -= 1;
            if (currentPosition <= -1)
                currentPosition = sceneList.Count - 1;      // 끝일때 맞은편으로 이동 TODO:
            ActiveCurrentScene();
            return;
        }
        if (direction == "RIGHT")
        {
            currentPosition += 1;
            if (currentPosition >= sceneList.Count)
                currentPosition = 0;                        // 끝일때 맞은편으로 이동 TODO:
            ActiveCurrentScene();
            return;
        }
    }
}
