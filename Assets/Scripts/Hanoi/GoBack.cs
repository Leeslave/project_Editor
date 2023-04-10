using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoBack : MonoBehaviour
{
    public bool IsActive = false;
    public HanoiManager HM;
    public Color Af;
    public Color Bf;
    Image sr;
    EventTrigger ET;

    private void Awake()
    {
        sr = GetComponent<Image>();
        EventTrigger ET = GetComponent<EventTrigger>();
        AddEvent(ET, EventTriggerType.PointerEnter, OnPoint);
        AddEvent(ET, EventTriggerType.PointerExit, OutPoint);
        AddEvent(ET, EventTriggerType.PointerClick, Click);
    }
    void OnPoint(PointerEventData Data)
    {
        if (IsActive) sr.color = Af;
    }
    public void OutPoint(PointerEventData Data)
    {
        if (IsActive) sr.color = Bf;
    }
    void Click(PointerEventData A)
    {
        if (name == "Go") HM.GoEvent();
        else HM.BackEvent();
        
    }



    void AddEvent(EventTrigger eventTrigger, EventTriggerType Type, Action<PointerEventData> Event)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = Type;
        entry.callback.AddListener((data) => { Event((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }
}
