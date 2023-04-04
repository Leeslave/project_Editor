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
    *   업무 착수 및 관리
        - 당일 할당된 업무들 플레이
    *   메일 및 기타 PC 업무
    */
    
    private GameObject worldObject;   //스크린이 활성화 된 world

    /// 스크린내 기본 요소 (에디터 할당)
    public GameObject screen;      // 스크린
    public GameObject bootPanel;    //부팅 패널
    public GameObject desktop;      // 바탕화면 패널

    /// 부팅 패널 요소 (find)
    private GameObject bootLogo;    //로고 이미지
    private GameObject bootCLI;     //부팅 콘솔 텍스트창

    /// 부팅 애니메이션 설정값
    public AnimationController bootAnimation;   //부팅 애니메이션
    public float logoOnSeconds;     //로고 이미지 활성 시간
    private string currentBootStatus;        //현 부팅 상태 (Off > On > TryOff> )

    /// 컴포넌트 설정, 부팅 초기값 off
    private void Start() {
        /// 월드 오브젝트 할당
        worldObject = GameObject.FindObjectOfType<WorldCanvas>().gameObject;

        /// 부팅 오브젝트 할당
        bootLogo = bootPanel.transform.GetChild(0).gameObject;
        bootCLI = bootPanel.transform.GetChild(1).gameObject;
        bootCLI.GetComponent<Text>().text = "";

        /// 부팅 대기 화면으로 설정
        desktop.SetActive(false);
        bootPanel.SetActive(true);
        bootLogo.SetActive(false);
        bootCLI.SetActive(false);

        currentBootStatus = "Off";
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
            case "Off":
                StartCoroutine("BootScreen");
                break;
            case "On":
                StartCoroutine("OffScreen");
                break;
            case "TryOff":
                currentBootStatus = "GetOff";
                break;
        }
    }

    /// <summary>
    /// 재부팅 버튼 클릭 이벤트
    /// </summary>
    public void OnResetClicked()
    {
        StopAllCoroutines();
        bootCLI.GetComponent<Text>().text = "";
        StartCoroutine("BootScreen");
    }

    /**
    * 스크린 부팅 싱글톤
    * - 로고 -> 부팅 콘솔 -> 스크린 On
    */
    IEnumerator BootScreen()
    {
        // 부팅 시작
        currentBootStatus = "TryOn";
        bootPanel.SetActive(true);
        desktop.SetActive(false);

        // 로고 활성화
        bootLogo.SetActive(true);
        bootCLI.SetActive(false);
        yield return new WaitForSeconds(logoOnSeconds);

        // 부팅 콘솔 텍스트 활성화
        bootLogo.SetActive(false);
        bootCLI.SetActive(true);
        bootAnimation.Play();
        yield return new WaitUntil(() => bootAnimation.isFinished == true);

        // 부팅 완료 (콘솔창 초기화)
        yield return new WaitForSeconds(0.6f);
        bootCLI.GetComponent<Text>().text = "";
        bootPanel.SetActive(false);
        currentBootStatus = "On";

        desktop.SetActive(true);
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
        currentBootStatus = "TryOff";
        desktop.SetActive(false);
        bootPanel.SetActive(true);
        bootLogo.SetActive(false);
        bootCLI.SetActive(true);

        // 3초 내 종료 대기
        bootCLI.GetComponent<Text>().text = "\n\n3초 내로 전원 버튼을 다시 눌러 전원 끄기...";
        for(var i = 0; i<6; i++)
        {
            // 종료
            if(currentBootStatus == "GetOff")
            {
                currentBootStatus = "Off";
                // TODO: desktop 초기화 (종료)
                worldObject.SetActive(true);
                bootCLI.GetComponent<Text>().text = "";
                bootPanel.transform.parent.gameObject.SetActive(false);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }

        // 종료 취소 및 스크린 재개
        currentBootStatus = "On";
        bootCLI.GetComponent<Text>().text = "";
        desktop.SetActive(true);
        bootPanel.SetActive(false);
    }
}
