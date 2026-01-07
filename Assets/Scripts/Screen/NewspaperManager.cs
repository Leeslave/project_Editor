using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewspaperManager : MonoBehaviour
{
    [SerializeField] private GameObject newsWindow;

    // Start is called before the first frame update
    void Start()
    {
        newsWindow.SetActive(false);    
    }

    public void ActivateWindow() 
    {
        if (!newsWindow.activeSelf) { newsWindow.SetActive(true); }
    }

    public void CloseWindow()
    {
        if (newsWindow.activeSelf) { newsWindow.SetActive(false); }
    }

    /* 각 일자별로 뉴스 내용 이미지 만들어서 각 날짜에 맞는 이미지 불러와서 출력 기능 추가 */
}
