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
    public int year;
    public int month;
    public int day;
    public int time;


    private void Start() {
        cameraManager = Camera.main.GetComponent<CameraMove>();
        cameraManager.enabled = false;
        awakeSFX = GetComponent<AudioSource>();
        dayText.GetComponent<TMP_Text>().text = $"제국력 {year}년 {month}월 {day}일\n\n{time}:00";
        StartCoroutine("DayCountIntro");
    }

    IEnumerator DayCountIntro() {
        dayText.SetActive(false);
        yield return new WaitForSeconds(textOnDelay);
        dayText.SetActive(true);
        awakeSFX.Play();
        yield return new WaitForSeconds(textOnDuration);
        gameObject.SetActive(false);
        cameraManager.enabled = true;
    }
}
