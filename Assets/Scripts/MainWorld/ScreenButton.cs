using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenButton : MonoBehaviour
{
    /**
    *   스크린 활성화, 비활성화 트리거 코드
    *   - 스크린 아이콘을 눌러 스크린을 활성화
    *   - down 버튼을 눌러 스크린 비활성화
    */
    public GameObject downButton;   // 취소 버튼 (아래 버튼)
    private ScreenManager screenObject; // 스크린 매니저
    public float screenZoomMultiplier;  // 스크린 아이콘 확대 배율

    private bool isScreenZoomed;

    private void Awake() {
        isScreenZoomed = false;
        downButton.SetActive(false);
        screenObject = GameObject.FindObjectOfType<ScreenManager>();
        if (screenObject == null)
        {
            Debug.LogError("Cannot Found Screen");
        }
    }

    /// <summary>
    /// 스크린을 눌러 확대 or 스크린 키기
    /// </summary>
    public void ClickScreen()
    {
        // 스크린 확대 전 -> 확대하기, 버튼 활성화 전환
        if (isScreenZoomed == false)
        {
            // 현재 씬 내 스크린 버튼 제외 전부 비활성화
            Transform currentSceneObject = transform.parent;
            for (int idx = 0; idx < currentSceneObject.childCount; idx++)
            {
                if (currentSceneObject.GetChild(idx) == transform)
                    continue;
                currentSceneObject.GetChild(idx).gameObject.SetActive(false);
            }
            // 축소 버튼 활성화
            downButton.SetActive(true);
            isScreenZoomed = true;

            // 확대
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = rectTransform.localScale * screenZoomMultiplier;
            return;
        }

        // 스크린 확대 후 -> 스크린 키기, 이전 캔버스 비활성화
        else
        {
            screenObject.worldObject = transform.parent.parent.GetComponent<WorldCanvas>();    // 월드 캔버스
            screenObject.SetScreen(ScreenManager.ScreenMode.Off);
            transform
                .parent         //월드 페이지들
                .parent.gameObject.SetActive(false);    // 월드 캔버스
        }
    }

    /// <summary>
    /// 활성화 스크린을 축소
    /// </summary>
    public void DeactivateScreen()
    {
        if (isScreenZoomed == true)
        {
            Transform roomObject = transform.parent;
            for (int idx = 0; idx < roomObject.childCount; idx++)
            {
                if (roomObject.GetChild(idx) == gameObject)
                    continue;
                roomObject.GetChild(idx).gameObject.SetActive(true);
            }
            downButton.SetActive(false);

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = rectTransform.localScale / screenZoomMultiplier;
            isScreenZoomed = false;
        }
    }
}
