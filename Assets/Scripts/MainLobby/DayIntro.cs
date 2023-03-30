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
    private PlayerDataManager playerDataManager;  //TODO: 월드 로딩 용 데이터매니저

    public float textOnDelay;   // 시작부터 글자 활성화까지의 딜레이
    public float textOnDuration;    //글자 활성화 지속시간
    public GameObject dayText;      //글자 활성화용 텍스트 오브젝트
    private string hour;

    private void setHour(int timeOffset)
    {
        switch(timeOffset) {
            case 1:
                hour = "09:00 AM";
                break;
            case 2:
                hour = "05:00 PM";
                break;
            case 3:
                hour = "07:30 PM";
                break;
            case 0:default:
                hour = "06:30 AM";
                break;
        }
    }

    private void Awake()
    {
        awakeSFX = GetComponent<AudioSource>();
        playerDataManager = GameObject.FindObjectOfType<PlayerDataManager>();
    }

    private void Start() 
    {
        Debug.Log("Current Renown: " + PlayerPrefs.GetInt("Renown").ToString());
        StartCoroutine("DayCountIntro");
    }    

    IEnumerator DayCountIntro() {
        dayText.SetActive(false);
        setHour(PlayerPrefs.GetInt("Time"));
        dayText.GetComponent<TMP_Text>().text = $"제국력 {PlayerPrefs.GetInt("Year")}년 {PlayerPrefs.GetInt("Month")}월 {PlayerPrefs.GetInt("Day")}일\n\n{hour}";
        yield return new WaitForSeconds(textOnDelay);
        dayText.SetActive(true);
        awakeSFX.Play();
        yield return new WaitForSeconds(textOnDuration);
        gameObject.SetActive(false);
           // TODO: 위치에 해당하는 worldCanvas 생성으로 수정
    }
}
