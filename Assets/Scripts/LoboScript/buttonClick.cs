using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class buttonClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject MainCamera;
    public GameObject Screen;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.ToString() + "Clicked!");
    }

    /// <summary>
    /// 버튼 클릭시 반응
    /// </summary>

    private void OnMouseDown()
    {
        // 스크린 활성화 및 이동, 스크린 활성화/비활성화 전환
        if (gameObject.CompareTag("Screen"))
        {
            //MainCamera.GetComponent<CameraMove>().MoveToScreen();
            Screen.GetComponent<UIObjectManagement>().ObjectOnOff();
        }
    }
}
