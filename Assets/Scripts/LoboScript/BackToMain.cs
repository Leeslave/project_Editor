using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMain : MonoBehaviour
{
    public GameObject MainCamera;

    /// <summary>
    /// 메인 화면으로 돌아가기
    /// - 스크린 비활성화, 카메라 위치 돌려놓기
    /// </summary>
    public void backToMain()
    {
        gameObject.SetActive(false);
        //GameObject.FindWithTag("Screen").GetComponent<MainSceneManager>().MainUIOn();
        MainCamera.GetComponent<CameraMove>().MoveToMain();
    }
}
