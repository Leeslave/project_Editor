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
    public string locationName;     //해당 World의 위치명
    public List<Image> sceneList;    // 배경컷 리스트

    [SerializeField]
    private int currentPosition = 0;    // 현재 배경컷 위치


    /// 이벤트 카메라 할당
    private void Awake() {
        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void Start() {
        asyncWorldTime();
        ActiveScene();
    }
    /// 활성화될때마다 월드 내 시간 동기화
    void OnEnabled()
    {
        asyncWorldTime();
        ActiveScene();
    }

    /// <summary>
    /// 시간대 동기화로 배경 이미지 설정 및 위치활성화
    /// </summary>
    /// <remarks>- "이름_순서_시간대.png"</remarks>
    /// <remarks>- 다른 파일 없으면 기본 값 유지 </remarks>
    public void asyncWorldTime()
    {
        for(int idx = 0; idx < transform.childCount; idx++)
        {
            Sprite newImage = Resources.Load<Sprite>($"{locationName}_{idx}_{PlayerDataManager.Instance.playerData.time}.png");
            if (newImage != null)
                sceneList[idx].sprite = newImage;
            // 날짜/시간별 특수 오브젝트 활성화, 비활성화
        }
    }

    /// <summary>
    /// 현재 위치 활성화
    /// </summary>
    public void ActiveScene()
    {
        for(int idx = 0; idx < transform.childCount; idx++)
        {
            sceneList[idx].gameObject.SetActive(idx == currentPosition);
        }
    }

    /// <summary>
    /// 모든 씬 페이지 비활성화
    /// </summary>
    public void DeactiveScene()
    {
        for(int idx = 0; idx < transform.childCount; idx++)
        {
            sceneList[idx].gameObject.SetActive(false);
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
                ActiveScene();
                return;
            case "RIGHT":
                if (currentPosition < sceneList.Count)
                    currentPosition += 1; 
                ActiveScene();
                return;
            // TODO: case UP,DOWN
        }
    }
}
