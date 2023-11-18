using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoBack : MonoBehaviour
{
    // 현재 버튼의 활성화 여부
    public bool IsActive = false;
    public HanoiManager HM;
    // 하이라이트 됬을 떄, 되지 않았을 떄의 색
    public Color Af;
    public Color Bf;
    // Image Component를 저장할 변수
    Image sr;
    // EventTrigger Component를 저장할 변수
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
        if (!HM.TouchAble||HM.IsPick) return;
        if (IsActive) sr.color = Af;
    }
    public void OutPoint(PointerEventData Data)
    {
        if (!HM.TouchAble|| HM.IsPick) return;
        if (IsActive) sr.color = Bf;
    }

    // 되돌리기 버튼이면 HanoiManager의 BackEvent를
    // 다시실행 버튼이면 HanoiManager의 GoEvent를 실행한다.
    void Click(PointerEventData A)
    {
        if (!HM.TouchAble || HM.IsPick) return;
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
