using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvas : MonoBehaviour
{
    /**
    * 로비의 location내의 기능 관리
        현재 위치를 기반으로
        - 시간대 변경 -> 배경 이미지 변경
        - 위치 내 이동 -> 카메라 이동, 좌우버튼
        - 다음 위치로 이동하기 -> 오브젝트 파괴, 다음 위치 프리팹 생성
        - 현재 위치 활성화 -> 첫 로드때, 컷씬&스크린에서 돌아오기
    */
    public string location;     //해당 World의 위치명

    public List<Image> sceneList;    // 배경컷 리스트

    private int currentPosition = 0;    // 현재 배경컷 위치

    /**
    * 시간대 동기화로 배경 이미지 설정
    *   - "이름_순서_시간대.png"
    *   - 현재 위치 활성화
    */
    public void asyncWorldTime(int time)
    {
        for(int idx = 0; idx < transform.childCount; idx++)
        {
            Sprite newImage = Resources.Load<Sprite>($"{location}_{idx}_{time}.png");
            if (newImage != null)
                sceneList[idx].sprite = newImage;
        }

        ActiveCurrentScene();
    }

    private void ActiveCurrentScene()
    {
        for(int idx = 0; idx < transform.childCount; idx++)
        {
            sceneList[idx].gameObject.SetActive(idx == currentPosition);
        }
    }

    /// <summary>
    /// 카메라 위치 옮기기
    /// - 좌우 : 옆 화면으로 이동하기(리스트로 관리)
    /// - 아래 : 이전 화면으로 이동 (축소 ,,,)
    /// - 위 : 미정
    /// </summary> 
    public void MoveCamera(string direction)
    {
        switch(direction)
        {
            case "LEFT":
                currentPosition -= 1;
                if (currentPosition <= -1)
                    currentPosition = sceneList.Count - 1;      // 끝일때 맞은편으로 이동 TODO:
                ActiveCurrentScene();
                return;
            case "RIGHT":
                currentPosition += 1;
                if (currentPosition >= sceneList.Count)
                    currentPosition = 0;                        // 끝일때 맞은편으로 이동 TODO:
                ActiveCurrentScene();
                return;
            // TODO: case UP,DOWN
        }
    }
}
