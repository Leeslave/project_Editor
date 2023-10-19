using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    /**
    * 스크린 관리 스크립트
    *   스크린 전원 키고 끄기
        - 전원 켜기 (부팅 애니메이션)
        - 전원 끄기 (한번 더 눌러서 전원 끔)
        - 리셋 버튼 (초기화 밑 재부팅)
    *   desktop 관리
        - 로그인 기능 (TODO)
        - 특수 컷씬 기능 (+a)
    */

    /// 스크린내 기본 요소 (에디터 할당)
    public GameObject returnButton;     // 스크린 탈출 버튼
    public GameObject desktop;      // 바탕화면 패널
    public GameObject bootPanel;    //부팅 패널
    public Text bootCLI;     //부팅 콘솔 텍스트
    [SerializeField]
    private ScreenMode currentBootStatus;    //현 부팅 상태

    /// 부팅 애니메이션 설정값
    public AnimationController bootAnimation;   //부팅 애니메이션
    public float logoOnSeconds;     //로고 이미지 활성 시간

    public float powerOffDelay;   // 종료 대기 시간

    /// 스크린 모드
    public enum ScreenMode{
        Off,
        OnBoot,
        On,
        TryOff
    }

    // 싱글턴
    private static ScreenManager _instance;
    public static ScreenManager Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        // 싱글턴 설정
        if(!_instance)
        {
            _instance = this;

            // 현재 스크린 상태 설정
            if(GameSystem.Instance.isScreenOn)
            {
                /// 바탕 화면으로 설정
                SetScreen(ScreenMode.On);
            }
            else
            {
                /// 부팅 대기 화면으로 설정
                SetScreen(ScreenMode.Off);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 스크린 현재 모드 설정
    /// </summary>
    /// <remarks> 부팅 전으로 스크린 화면 활성화하기</remarks>
    /// <param name=screenMode>전환할 스크린 모드</param>
    public void SetScreen(ScreenMode screenMode) 
    {
        if (screenMode == ScreenMode.On)
        {
            //바탕화면 활성화
            desktop.SetActive(true);
            // 부팅패널 비활성화
            bootPanel.SetActive(false);
            // 탈출 버튼 비활성화
            returnButton.SetActive(false);
        }
        else
        {
            // 탈출 버튼 활성화
            returnButton.SetActive(true);
            // 바탕화면 비활성화
            desktop.SetActive(false);
            // 부팅패널 활성화
            bootPanel.SetActive(true);
            bootCLI.text = "";
            bootCLI.gameObject.SetActive(true);
        }
        currentBootStatus = screenMode;
    }

    /// <summary>
    /// 전원 버튼 클릭 이벤트
    /// </summary>
    /// <remarks>
    /// <para>- Off -> On : 전원 키기</para>
    /// <para>- On -> TryOff -> GetOff -> Off : 전원 끄기 -> 한번 더 눌러 전원 끄기</para>
    /// </remarks>
    public void OnPowerClicked()
    {
        switch (currentBootStatus)
        {
            case ScreenMode.Off:
                StartCoroutine("BootScreen");
                break;
            case ScreenMode.On:
                StartCoroutine("OffScreen");
                break;
            case ScreenMode.TryOff:
                SetScreen(ScreenMode.Off);
                break;
        }
    }

    /// <summary>
    /// 재부팅 버튼 클릭 이벤트
    /// </summary>
    public void OnResetClicked()
    {
        if (currentBootStatus != ScreenMode.On)
            return;
        StopAllCoroutines();
        StartCoroutine("BootScreen");
    }

    /**
    * 스크린 부팅 싱글톤
    * - 로고 -> 부팅 콘솔 -> 스크린 On
    */
    IEnumerator BootScreen()
    {
        // 부팅 시작
        currentBootStatus = ScreenMode.OnBoot;
        SetScreen(ScreenMode.OnBoot);

        // 로고 활성화 후 종료
        bootPanel.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(logoOnSeconds);
        bootPanel.transform.GetChild(0).gameObject.SetActive(false);

        // 부팅 콘솔 텍스트 활성화
        bootAnimation.Play();
        yield return new WaitUntil(() => bootAnimation.isFinished == true);
        yield return new WaitForSeconds(0.6f);

        // 부팅 완료 (콘솔창 초기화)
        SetScreen(ScreenMode.On);
        GameSystem.Instance.isScreenOn = true;
        // TODO: desktop 초기화 (시작)
    }

    /**
    * 스크린 종료 싱글톤
    * - 종료 시도 -> 부팅 종료 콘솔 활성화 -> 버튼 다시 클릭 확인 후 종료
    *                                    -> 버튼 안누를 시 돌아감
    */
    IEnumerator OffScreen()
    {
        // 종료 시도
        SetScreen(ScreenMode.TryOff);

        // 종료 대기
        bootCLI.text = $"\n\n${(int)powerOffDelay}초 내로 전원 버튼을 다시 눌러 전원 끄기...";

        yield return new WaitUntil(() => currentBootStatus == ScreenMode.Off || Time.time >= powerOffDelay);

        // 전원 종료시
        if(currentBootStatus == ScreenMode.Off)
        {
            SetScreen(ScreenMode.Off);
            GameSystem.Instance.isScreenOn = false;
            // TODO: desktop 초기화 (종료)
        }
        // 종료 취소시
        else
        {
            SetScreen(ScreenMode.On);
        }
    }

    public void OnReturnClicked(string sceneName)
    {
        if (sceneName == null || sceneName == "")
            sceneName = "MainWorld";
        SceneManager.LoadScene(sceneName);
    }
}
