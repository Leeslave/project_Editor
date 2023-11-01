using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


/// <summary>
/// Maze의 Scene이동을 담당하는 Script
/// ToDo : 싱글턴 방식이 완벽히 정해지면 갈아 엎을 예정
/// </summary>
public class Maze_Scene : MonoBehaviour
{
    public TMP_Text Text;
    string SceneName;
    private void Awake()
    {
        if (name == "EXIT") { SceneName = "TestT";}
        else { 
            SceneName = "Maze";
            String Path = "Assets\\Resources\\GameData\\Maze";
            Text.text = Directory.GetFiles(Path)[0][Path.Length + 1].ToString();
        }
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
        SceneManager.LoadScene(SceneName);
    }
}
