using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    public WorldCanvas worldObject;   //스크린이 활성화 된 world

    /// 스크린내 기본 요소 (에디터 할당)
    public GameObject desktop;      // 바탕화면 패널
    public GameObject bootPanel;    //부팅 패널
    private Text bootCLI;     //부팅 콘솔 텍스트
    private ScreenMode currentBootStatus;    //현 부팅 상태

    /// 부팅 애니메이션 설정값
    public AnimationController bootAnimation;   //부팅 애니메이션
    public float logoOnSeconds;     //로고 이미지 활성 시간

    /// 스크린 모드
    public enum ScreenMode{
        Off,
        OnBoot,
        On,
        TryOff
    }

    // 싱글톤
    private static ScreenManager _instance;
    public static ScreenManager Instance
    {
        get { return _instance; }
    }
    void Awake()
    {
        if(!_instance)
        {
            _instance = this;
            bootCLI = bootPanel.transform.GetChild(1).GetComponent<Text>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// 스크린 초기 상태 설정
    /// 컴포넌트 설정, 부팅 초기값 off
    void Start() 
    {
        /// 부팅 대기 화면으로 설정
        desktop.SetActive(false);
        bootPanel.SetActive(true);
        bootCLI.text = "";
        bootCLI.gameObject.SetActive(false);

        currentBootStatus = ScreenMode.Off;
    }

    /// 

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
        }
        else
        {
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
        if (currentBootStatus == ScreenMode.On)
        {
            StopAllCoroutines();
            StartCoroutine("BootScreen");
        }
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

        // 3초 내 종료 대기
        bootCLI.text = "\n\n3초 내로 전원 버튼을 다시 눌러 전원 끄기...";
        for(var i = 0; i<6; i++)
        {
            // 종료
            if(currentBootStatus == ScreenMode.Off)
            {
                SetScreen(ScreenMode.Off);
                // TODO: desktop 초기화 (종료)
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }

        // 종료 취소 및 스크린 재개
        SetScreen(ScreenMode.On);
    }
}
