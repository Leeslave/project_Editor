using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
* WorldCanvas 내 기능
*   월드 내 이동
*   날짜 변경시 월드 동기화
    - 배경 이미지
    - NPC들 대사 파일
*/
public class WorldCanvas : MonoBehaviour
{
    public string locationName;     //해당 World의 위치명
    [SerializeField]
    public List<Image> imageList;    // 배경컷 리스트

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
    void OnEnable()
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
            Sprite newImage = Resources.Load<Sprite>($"{locationName}_{idx}_{GameSystem.Instance.player.time}.png");
            if (newImage != null)
                imageList[idx].sprite = newImage;
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
            imageList[idx].gameObject.SetActive(idx == currentPosition);
        }
    }

    /// <summary>
    /// 모든 씬 페이지 비활성화
    /// </summary>
    public void DeactiveScene()
    {
        for(int idx = 0; idx < transform.childCount; idx++)
        {
            imageList[idx].gameObject.SetActive(false);
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
                if (currentPosition < imageList.Count - 1)
                    currentPosition += 1; 
                ActiveScene();
                return;
            // TODO: case UP,DOWN
        }
    }

    /// <summary>
    /// 월드 이동 이벤트함수
    /// </summary>
    public void MoveWorldDoor(string nextWorld)
    {
        transform.parent.GetComponent<WorldSceneManager>().asyncLocation(nextWorld);
    }
}
