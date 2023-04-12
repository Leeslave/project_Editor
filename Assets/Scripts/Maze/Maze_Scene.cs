using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Maze_Scene : MonoBehaviour
{
    string SceneName;
    private void Awake()
    {
        if (name == "EXIT") { SceneName = "TestT"; PlayerPrefs.SetString("Clear", "N"); }
        else { SceneName = "Maze"; }
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
        SceneManager.LoadScene(SceneName);
    }
}
