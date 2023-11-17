using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ExitBT : MonoBehaviour
{
    private void Awake()
    {
        if(PlayerPrefs.HasKey("Clear")) PlayerPrefs.SetString("Clear", "N");
        AddEvent(GetComponent<EventTrigger>(), EventTriggerType.PointerClick, NextScene);
    }
    void AddEvent(EventTrigger eventTrigger, EventTriggerType Type, Action<PointerEventData> Event)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = Type;
        entry.callback.AddListener((data) => { Event((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }
    void NextScene(PointerEventData Data)
    {
        SceneManager.LoadScene("TestT");
    }
}
