using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayIntro : MonoBehaviour
{
    /**
    *   하루 시작 인트로 스크립트 (PlayerPrefs 갱신 직후 실행)
        : 씬이 재로딩 될때마다(날짜 변경)마다 실행
    *   - 날짜 인트로 애니메이션 첫 활성화
        - PlayerDataManager의 월드 로딩 실행
    */
    private AudioSource awakeSFX;   // 인트로용 효과음
    public float textOnDelay;   // 시작부터 글자 활성화까지의 딜레이
    public float textOnDuration;    //글자 활성화 지속시간
    public TMP_Text dayText;      //글자 활성화용 텍스트 오브젝트

    /**
    * 시간대 문자열 설정 함수
    - (0: 출근전, 1: 업무전, 2: 업무후, 3: 퇴근 후)
    */ 
    private string setHour(int timeOffset)
    {
        switch(timeOffset) {
            case 1:
                return "09:00 AM";
            case 2:
                return "05:00 PM";
            case 3:
                return "07:30 PM";
            case 0:default:
                return "06:30 AM";
        }
    }

    // 컴포넌트 불러오기
    private void Awake()
    {
        awakeSFX = GetComponent<AudioSource>();
    }

    // 인트로 시작
    private void Start() 
    {
        Debug.Log("Current Renown: " + PlayerDataManager.Instance.playerData.renown);
        StartCoroutine("DayCountIntro");
    }    

    /**
    * 날짜 출력 인트로
    *   - 날짜 텍스트 출력
    *   - Intro 비활성화 하면서 현재 WorldCanvas 설정하는 함수 호출
    */
    IEnumerator DayCountIntro() {
        dayText.gameObject.SetActive(false);

        PlayerData playerData = PlayerDataManager.Instance.playerData;

        // 텍스트 세팅
        dayText.text = $"제국력 {playerData.date.year}년 {playerData.date.month}월 {playerData.date.day}일\n\n{setHour(playerData.time)}";
        yield return new WaitForSeconds(textOnDelay);

        //날짜 활성화 애니메이션, 효과음
        dayText.gameObject.SetActive(true);
        if(awakeSFX != null) awakeSFX.Play();
        yield return new WaitForSeconds(textOnDuration);

        //종료 및 WorldCanvas 설정
        FindObjectOfType<WorldSceneManager>().asyncWorldCanvas();
        gameObject.SetActive(false);
    }
}
