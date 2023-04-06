using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RealEnd : MonoBehaviour
{
    string ch;

    // 엔딩
    // Input
    // a1 : GameOver인지, GameClear인지
    // a2 : 엔딩 시 밑에 생성되는 문구
    public void Ending(string a1, string a2)
    {
        transform.GetChild(1).GetComponent<TMP_Text>().text = a1;
        ch = a2;
        StartCoroutine(EEE());
    }

    // 엔딩 시 연출
    IEnumerator EEE()
    {
        yield return new WaitForSeconds(2.5f);

        // GameOver or GameClear
        TMP_Text a1 = transform.GetChild(1).GetComponent<TMP_Text>();
        // 엔딩 시 밑에 생성되는 문구
        TMP_Text a2 = transform.GetChild(2).GetComponent<TMP_Text>();

        // FadeIn 연출에 사용
        for (; a1.color.a < 1;)
        {
            a1.color = new Color(1, 1, 1,a1.color.a + 0.01f);
            yield return new WaitForSeconds(0.02f);
        }

        // 타자 효과 연출에 사용
        foreach(var a in ch)
        {
            yield return new WaitForSeconds(0.5f);
            a2.text += a;
        }

        // 종료 버튼 생성
        transform.GetChild(3).gameObject.SetActive(true);

        // 종료 버튼에 종료 기능 할당
        MyUi.AddEvent(transform.GetChild(3).gameObject.GetComponent<EventTrigger>(), EventTriggerType.PointerClick,Exit);

        yield break;
    }

    // 종료 버튼
    void Exit(PointerEventData a)
    {
        Debug.Log("!");
        Application.Quit();
    }
}
