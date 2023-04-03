using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RealEnd : MonoBehaviour
{
    string ch;
    public void Ending(string a1, string a2)
    {
        transform.GetChild(1).GetComponent<TMP_Text>().text = a1;
        ch = a2;
        StartCoroutine(EEE());
    }

    IEnumerator EEE()
    {
        yield return new WaitForSeconds(2.5f);
        TMP_Text a1 = transform.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text a2 = transform.GetChild(2).GetComponent<TMP_Text>();
        for (; a1.color.a < 1;)
        {
            a1.color = new Color(1, 1, 1,a1.color.a + 0.01f);
            yield return new WaitForSeconds(0.02f);
        }

        foreach(var a in ch)
        {
            yield return new WaitForSeconds(0.5f);
            a2.text += a;
        }

        transform.GetChild(3).gameObject.SetActive(true);
        AddEvent(transform.GetChild(3).gameObject.GetComponent<EventTrigger>(), EventTriggerType.PointerClick,Exit);

        yield break;
    }

    void Exit(PointerEventData a)
    {
        Debug.Log("!");
        Application.Quit();
    }

    void AddEvent(EventTrigger eventTrigger, EventTriggerType Type, Action<PointerEventData> Event)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = Type;
        entry.callback.AddListener((data) => { Event((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }
}
