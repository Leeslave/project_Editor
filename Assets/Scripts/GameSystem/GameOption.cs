using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameOption : Singleton<GameOption>
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

    /// 해상도 설정
    // TODO

    // 볼륨 설정
    [SerializeField]
    private AudioMixer audioMixer;
    
    new void Awake()
    {
        base.Awake();
        // Screen.SetResolution(resolutionX, resolutionY, false);  // 해상도 고정
    }
    
    public Button optionButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(optionButton is not null)
            {
                optionButton.onClick.Invoke();
            }
        }
    }

    
    /// <summary>
    /// 배경음악 볼륨 조절 함수
    /// </summary>
    /// <param name="volume">조절할 볼륨 수치</param>
    public void SetBGMVolume(float volume)
    {
        // decibel 단위로 변환
        float dB;
        if (volume > 0.0001f)
            dB = Mathf.Log10(volume) * 20;
        else
            dB = -80f; // 음소거 상태로 설정

        audioMixer.SetFloat("BGMVolume", dB);
    }
    
    /// <summary>
    /// 효과음 볼륨 조절 함수
    /// </summary>
    /// <param name="volume">조절할 볼륨 수치</param>\
    public void SetSFXVolume(float volume)
    {
        // decibel 단위로 변환
        float dB;
        if (volume > 0.0001f)
            dB = Mathf.Log10(volume) * 20;
        else
            dB = -80f; // 음소거 상태로 설정

        audioMixer.SetFloat("SFXVolume", dB);
    }

    
    public void OnBGMVolumeChanged(float value)
    {
        SetBGMVolume(value);
    }
    
    
    public void OnSFXVolumeChanged(float value)
    {
        SetSFXVolume(value);
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
