using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOption : MonoBehaviour
{
    /**
    게임 옵션 컨트롤러
    *   옵션 패널 활성화/비활성화
    *   메인메뉴로 돌아가기
    *   게임 종료하기
    *   TODO: 게임 옵션 설정
        - 볼륨
        - 해상도
        - 언어?
    */
    
    public Button optionButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(optionButton != null)
            {
                optionButton.onClick.Invoke();
            }
        }
    }


    public void LoadSaveSelect()
    {
        GameSystem.LoadScene("SaveSelect");
    }

    public void LoadStartMenu()
    {
        GameSystem.LoadScene("Start");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
