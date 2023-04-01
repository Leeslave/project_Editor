using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
* 로비의 location내의 기능 관리
    현재 위치를 기반으로
    
    (활성화될때마다 자동으로)
    - 시간대 변경 -> 배경 이미지 변경
    - 현재 위치 활성화 -> 첫 로드때, 컷씬&스크린에서 돌아오기

    - 위치 내 이동 -> 카메라 이동, 좌우버튼
    - 다음 위치로 이동하기 -> 오브젝트 파괴, 다음 위치 프리팹 생성
*/
public class WorldCanvas : MonoBehaviour
{
    public string location;     //해당 World의 위치명

    public List<Image> sceneList;    // 배경컷 리스트

    private int currentPosition = 0;    // 현재 배경컷 위치

    /// 활성화될때마다 월드 내 시간 동기화
    void OnEnabled()
    {
        asyncWorldTime();
        ActiveCurrentScene();
    }

    /// <summary>
    /// 시간대 동기화로 배경 이미지 설정 및 위치활성화
    /// </summary>
    /// <remarks>- "이름_순서_시간대.png"</remarks>
    private void asyncWorldTime()
    {
        for(int idx = 0; idx < transform.childCount; idx++)
        {
            Sprite newImage = Resources.Load<Sprite>($"{location}_{idx}_{PlayerPrefs.GetInt("Time")}.png");
            if (newImage != null)
                sceneList[idx].sprite = newImage;
        }
    }

    /**
    * 월드 내 위치로 현재 화면 활성화
    */  
    private void ActiveCurrentScene()
    {
        for(int idx = 0; idx < transform.childCount; idx++)
        {
            sceneList[idx].gameObject.SetActive(idx == currentPosition);
        }
    }

    /// <summary>
    /// 카메라 위치 옮기기
    /// </summary>
    /// <param name="Direction">상하좌우 이동(Upper)</param>
    public void MoveCamera(string direction)
    {
        switch(direction)
        {
            case "LEFT":
                if (currentPosition > 0)
                    currentPosition -= 1;
                ActiveCurrentScene();
                return;
            case "RIGHT":
                if (currentPosition < sceneList.Count)
                    currentPosition += 1; 
                ActiveCurrentScene();
                return;
            // TODO: case UP,DOWN
        }
    }

    public void MoveNextWorld()
    {

    }
}
