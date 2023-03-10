using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateScreen : MonoBehaviour
{
    /**
    *   스크린 활성화, 비활성화 트리거 코드
    *   - 스크린 아이콘을 눌러 스크린을 활성화
    *   - down 버튼을 눌러 스크린 비활성화
    */
    public List<GameObject> offList;
    public GameObject downButton;
    public GameObject screenObject;
    public float screenZoomMultiplier;

    private bool isScreenActive;

    private void Awake() {
        isScreenActive = false;
        downButton.SetActive(false);
    }

    /// <summary>
    /// 스크린을 눌러 확대 or 스크린 키기
    /// </summary>
    public void ClickScreen()
    {
        // 스크린 확대 전 -> 확대하기, 버튼 활성화 전환
        if (isScreenActive == false)
        {
            foreach(var idx in offList)
            {
                idx.SetActive(false);
            }
            downButton.SetActive(true);
            isScreenActive = true;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = rectTransform.localScale * screenZoomMultiplier;
            return;
        }

        // 스크린 확대 후 -> 스크린 키기, 이전 캔버스 비활성화
        else
        {
            screenObject.SetActive(true);
            transform.parent.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 활성화 스크린을 축소
    /// </summary>
    public void DeactivateScreen()
    {
        if (isScreenActive == true)
        {
            foreach(var idx in offList)
            {
                idx.SetActive(true);
            }
            downButton.SetActive(false);

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = rectTransform.localScale / screenZoomMultiplier;
        }
    }
}
