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
    private SceneController cameraManager;

    
    public float textOnDelay;
    public float textOnDuration;
    public GameObject dayText;
    private string hour;

    private void Awake()
    {
        Debug.Log("Current Renown: " + PlayerPrefs.GetInt("Renown").ToString());

        awakeSFX = GetComponent<AudioSource>();
        cameraManager = Camera.main.GetComponent<SceneController>();
    }

    private void Start() {
        cameraManager.enabled = false;

        switch(PlayerPrefs.GetInt("Time")) {
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
        dayText.GetComponent<TMP_Text>().text = $"제국력 {PlayerPrefs.GetInt("Year")}년 {PlayerPrefs.GetInt("Month")}월 {PlayerPrefs.GetInt("Day")}일\n\n{hour}";
        yield return new WaitForSeconds(textOnDelay);
        dayText.SetActive(true);
        awakeSFX.Play();
        yield return new WaitForSeconds(textOnDuration);
        gameObject.SetActive(false);
        cameraManager.enabled = true;
    }
}
