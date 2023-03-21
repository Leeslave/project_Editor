using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayIntro : MonoBehaviour
{
    /**
    *   하루 시작 인트로 스크립트
    *   - 날짜 인트로
    */
    private AudioSource awakeSFX;
    private CameraMove cameraManager;

    
    public float textOnDelay;
    public float textOnDuration;
    public GameObject dayText;
    private int year;
    private int month;
    private int day;
    private int time;
    private string hour;

    private void Awake()
    {
        year = PlayerPrefs.GetInt("Year");
        month = PlayerPrefs.GetInt("Month");
        day = PlayerPrefs.GetInt("Day");
        time = PlayerPrefs.GetInt("Time");
        Debug.Log("Current Renown: " + PlayerPrefs.GetInt("Renown").ToString());

        awakeSFX = GetComponent<AudioSource>();
        cameraManager = Camera.main.GetComponent<CameraMove>();
    }

    private void Start() {
        cameraManager.enabled = false;

        switch(time) {
            case 0:
                hour = "06:30 AM";
                break;
            case 1:
                hour = "09:00 AM";
                break;
            case 2:
                hour = "05:00 PM";
                break;
            case 3:
                hour = "07:30 PM";
                break;
        }

        StartCoroutine("DayCountIntro");
    }

    IEnumerator DayCountIntro() {
        dayText.SetActive(false);
        dayText.GetComponent<TMP_Text>().text = $"제국력 {year}년 {month}월 {day}일\n\n{hour}";
        yield return new WaitForSeconds(textOnDelay);
        dayText.SetActive(true);
        awakeSFX.Play();
        yield return new WaitForSeconds(textOnDuration);
        gameObject.SetActive(false);
        cameraManager.enabled = true;
    }
}
