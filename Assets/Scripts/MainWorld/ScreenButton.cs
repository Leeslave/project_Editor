using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenButton : MonoBehaviour
{
    /**
    *   스크린 활성화, 비활성화 트리거 코드
    *   - 스크린 아이콘을 눌러 스크린을 활성화
    *   - down 버튼을 눌러 스크린 비활성화
    */
    public GameObject downButton;   // 취소 버튼 (아래 버튼)
    public float screenZoomMultiplier;  // 스크린 아이콘 확대 배율

    private bool isScreenZoomed;

    private void Awake() {
        isScreenZoomed = false;
        downButton.SetActive(false);
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

        // 스크린 확대 후 -> 스크린 활성화
        else
        {
            SceneManager.LoadScene("Screen");
        }
    }

    /// <summary>
    /// 활성화 스크린을 축소
    /// </summary>
    public void DeactivateScreen()
    {
        // 스크린 비활성화 시 return
        if (isScreenZoomed == false)
            return;

        // 현재 위치 오브젝트
        Transform roomObject = transform.parent;
        // 해당오브젝트 제외 모든 오브젝트 활성화
        for (int idx = 0; idx < roomObject.childCount; idx++)
        {
            if (roomObject.GetChild(idx) == gameObject)
                continue;
            roomObject.GetChild(idx).gameObject.SetActive(true);
        }
        // 축소 버튼 비활성화
        downButton.SetActive(false);

        // 스크린 오브젝트 축소
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale /= screenZoomMultiplier;
        isScreenZoomed = false;
    }
}
