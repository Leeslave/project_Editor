using System.Collections;
using UnityEngine;
using TMPro;

public class DayIntro : MonoBehaviour
{
    /**
    *   하루 시작 인트로 스크립트
        : 씬이 재로딩 될때마다(날짜 변경)마다 실행
    */
    private AudioSource _awakeSfx;   // 인트로용 효과음
    public float textOnDelay;   // 시작부터 글자 활성화까지의 딜레이
    public float textOnDuration;    //글자 활성화 지속시간
    public TMP_Text dayText;      //글자 활성화용 텍스트 오브젝트

    public bool isFinished;

    /**
    * 시간대 문자열 설정 함수
    - (0: 출근전, 1: 업무전, 2: 업무후, 3: 퇴근 후)
    */ 
    private string SetHour(int timeOffset)
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
        _awakeSfx = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 날짜 인트로 코루틴
    /// </summary>
    /// <returns></returns>
    public IEnumerator DayCountIntro()
    {
        // 플래그 설정
        isFinished = false;
        
        dayText.gameObject.SetActive(false);
        DailyData today = GameSystem.Instance.todayData;

        // 텍스트 세팅
        dayText.text = $"제국력 {today.date.year}년 {today.date.month}월 {today.date.day}일\n\n{SetHour(GameSystem.Instance.player.time)}";
        yield return new WaitForSeconds(textOnDelay);

        //날짜 활성화 애니메이션, 효과음
        dayText.gameObject.SetActive(true);
        if(_awakeSfx) _awakeSfx.Play();
        yield return new WaitForSeconds(textOnDuration);

        //종료 및 flag 설정
        gameObject.SetActive(false);
        isFinished = true;
    }
}
