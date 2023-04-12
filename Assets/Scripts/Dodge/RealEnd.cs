using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RealEnd : MonoBehaviour
{
    string ch;

    // ����
    // Input
    // a1 : GameOver����, GameClear����
    // a2 : ���� �� �ؿ� �����Ǵ� ����
    public void Ending(string a1, string a2)
    {
        transform.GetChild(1).GetComponent<TMP_Text>().text = a1;
        ch = a2;
        StartCoroutine(EEE());
    }

    // ���� �� ����
    IEnumerator EEE()
    {
        yield return new WaitForSeconds(2.5f);

        // GameOver or GameClear
        TMP_Text a1 = transform.GetChild(1).GetComponent<TMP_Text>();
        // ���� �� �ؿ� �����Ǵ� ����
        TMP_Text a2 = transform.GetChild(2).GetComponent<TMP_Text>();

        // FadeIn ���⿡ ���
        for (; a1.color.a < 1;)
        {
            a1.color = new Color(1, 1, 1,a1.color.a + 0.01f);
            yield return new WaitForSeconds(0.02f);
        }

        // Ÿ�� ȿ�� ���⿡ ���
        foreach(var a in ch)
        {
            yield return new WaitForSeconds(0.5f);
            a2.text += a;
        }

        // ���� ��ư ����
        transform.GetChild(3).gameObject.SetActive(true);

        // ���� ��ư�� ���� ��� �Ҵ�
        MyUi.AddEvent(transform.GetChild(3).gameObject.GetComponent<EventTrigger>(), EventTriggerType.PointerClick,Exit);

        yield break;
    }

    // ���� ��ư
    void Exit(PointerEventData a)
    {
        Debug.Log("!");
        Application.Quit();
    }
}
