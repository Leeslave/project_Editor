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
    
    public GameObject worldObject;   //스크린이 활성화 된 world
    public GameObject bootPanel;    //부팅 패널
    private GameObject bootLogo;    //로고 이미지
    private GameObject bootCLI;     //부팅 콘솔 텍스트창
    public AnimationController bootAnimation;   //부팅 애니메이션
    public float logoOnSeconds;     //로고 이미지 활성 시간
    private string currentBootStatus;        //현 부팅 상태 (Off > On > TryOff> )

    private void Start() {
        bootLogo = bootPanel.transform.GetChild(0).gameObject;
        bootCLI = bootPanel.transform.GetChild(1).gameObject;
        bootCLI.GetComponent<Text>().text = "";

        bootPanel.SetActive(true);

        bootLogo.SetActive(false);
        bootCLI.SetActive(false);

        currentBootStatus = "Off";
    }

    /**
    * 전원 버튼 클릭 이벤트
    * - Off -> On : 전원 키기
    * - On -> TryOff -> GetOff -> Off : 전원 끄기 -> 한번 더 눌러 전원 끄기
    */
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

    /**
    * 리셋 버튼 클릭 이벤트
    * - 즉시 재부팅 실행
    */
    public void OnResetClicked()
    {
        StopAllCoroutines();
        bootCLI.GetComponent<Text>().text = "";
        StartCoroutine("BootScreen");

        // TODO: desktop 초기화 후 시작
    }

    /**
    * 스크린 부팅 싱글톤
    * - 로고 -> 부팅 콘솔 -> 스크린 On
    */
    IEnumerator BootScreen()
    {
        currentBootStatus = "TryOn";
        bootPanel.SetActive(true);
        // TODO: desktop 비활성화

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

        // TODO: desktop 초기화 후 시작
    }

    /**
    * 스크린 종료 싱글톤
    * - 종료 시도 -> 부팅 종료 콘솔 활성화 -> 버튼 다시 클릭 확인 후 종료
    *                                    -> 버튼 안누를 시 돌아감
    */
    IEnumerator OffScreen()
    {
        currentBootStatus = "TryOff";
        // TODO: desktop 비활성화(정지)
        bootPanel.SetActive(true);
        bootLogo.SetActive(false);
        bootCLI.SetActive(true);

        bootCLI.GetComponent<Text>().text = "\n\n3초 내로 전원 버튼을 다시 눌러 전원 끄기...";
        for(var i = 0; i<6; i++)
        {
            if(currentBootStatus == "GetOff")
            {
                currentBootStatus = "Off";
                // TODO: desktop 비활성화(종료)
                worldObject.SetActive(true);
                bootCLI.GetComponent<Text>().text = "";
                bootPanel.transform.parent.gameObject.SetActive(false);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
        currentBootStatus = "On";
        bootCLI.GetComponent<Text>().text = "";
        // TODO: desktop 활성화(재개)
        bootPanel.SetActive(false);
    }
}
