using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public TMP_Text SceneName;
    private void Awake()
    {
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
        
        PlayerPrefs.SetString("Difficulty",name);
        SceneManager.LoadScene(SceneName.text);
    }
}
