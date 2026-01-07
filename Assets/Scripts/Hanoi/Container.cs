using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Container:MonoBehaviour
{
    // 하이라이트 됬을 때, 되지 않았을 때의 Sprite
    public Sprite NonClicked;
    public Sprite Clicked;
    // HanoiManager를 내장함수로 가지는 Object
    public HanoiManager HM;
    // 현재 Container에 쌓인 박스들의 리스트
    public List<GameObject> Boxes = new List<GameObject>();
    [HideInInspector]
    public int BoxSize = 0;
    // 현재 Container가 갖는 Index
    public int Ind;
    // Image Component를 담는데 사용될 변수
    Image sr;

    private void Awake()
    {
        sr = GetComponent<Image>();
        EventTrigger ET = GetComponent<EventTrigger>();
        AddEvent(ET,EventTriggerType.PointerEnter,OnPoint);
        AddEvent(ET,EventTriggerType.PointerExit, OutPoint);
        AddEvent(ET,EventTriggerType.PointerClick, Click);
    }

    // 포인터가 접근했을 때 호출되는 함수
    // Input : 호출된 상태에서 Pointer의 상태를 담은 Class(해당 함수에서는 미사용)
    void OnPoint(PointerEventData data)
    {
        if (!HM.TouchAble) return;
        // 현재 상자를 집은 상태고, 해당 상자를 현재 Container로 옮길 수 있다면, 하이라이트한다.
        if (HM.IsPick) { if (CompareTop(HM.PickedBox)) sr.sprite = Clicked; }
        // 현재 상자를 집지 않은 상태면 하이라이트한다.
        else
        {
            if (Boxes.Count != 0) 
            {
                if (!Boxes[Boxes.Count - 1].GetComponent<Box>().CanPick()) sr.sprite = NonClicked;
                else sr.sprite = Clicked;
            }
            else sr.sprite = NonClicked;
        }
    }
    // 포인터가 벗어났을 때 호출되는 함수
    // 포인터가 벗어나면 하이라이트를 중지한다.
    // Input : 호출된 상태에서 Pointer의 상태를 담은 Class(해당 함수에서는 미사용)
    public void OutPoint(PointerEventData data)
    {
        if (!HM.TouchAble) return;
        sr.sprite = NonClicked;
    }
    // 포인터로 해당 Object를 클릭했을 때 호출되는 함수.
    // Container간의 Box의 움직임을 담당한다.
    // Input : 호출된 상태에서 Pointer의 상태를 담은 Class(해당 함수에서는 미사용)
    void Click(PointerEventData data)
    {
        if (!HM.TouchAble) return;
        // 현재 상자를 집은 상태일 경우, Container와 해당 상자간의 상태를 비교하고 추가할 것인지 정한다.
        // HanoiManager.cs 참조
        if (HM.IsPick)
        {
            HM.AddEvent(Ind);
            HM._Init();
        }
        else
        {
            // 현재 상자를 집지 않은 경우
            // 비어있는 Container를 선택한 것이 아니라면 Container의 최상단 상자를 가져오며, 상자를 집은 상태로 바꾼다.
            HM.PickedBox = ReturnTop(0);
            if (HM.PickedBox != null)
            {
                HM.PickedBox.transform.SetAsLastSibling();
                HM.IsPick = true;
                HM.CurCon = Ind;
            }
        }
        OnPoint(null);
    }

    // 매게 변수로 들어온 상자와, 현재 Container간의 상태를 비교한다.
    // 비어있거나, 매개변수로 들어온 상자가 현재 Container 최상단의 상자보다 가볍다면 true를 반환하며 그렇지 않으면 false를 반환한다.
    // Input : 상자 Object
    // Output : Input으로 들어온 상자가 해당 Container에 추가될 수 있는지
    public bool CompareTop(GameObject A)
    {
        if (A == null) return true;
        if (Boxes.Count == 0) return true;
        if (A.GetComponent<Box>().CanAdd(Boxes[Boxes.Count - 1].GetComponent<Box>())) return true;
        else return false;
    }

    // 현재 Container의 최상단의 상자를 반환한다.
    // 비어있다면 null을 반환한다.
    // Output : 현재 Container 최상단의 상자.(비어있다면 null)
    public GameObject ReturnTop(int DuraCh)
    {
        if(Boxes.Count == 0) return null;
        else
        {
            GameObject cnt = Boxes[Boxes.Count - 1];
            if (DuraCh != 0) cnt.GetComponent<Box>().DuraChange(DuraCh);
            if (!cnt.GetComponent<Box>().CanPick()) return null;
            Boxes.RemoveAt(Boxes.Count - 1);
            BoxSize--;
            return cnt;
        }
    }

    // 현재 Container에 매개변수로 들어온 상자 Object를 쌓는다.
    // Input : 추가할 Box Object
    public void AddTop(GameObject cnt)
    {
        Vector3 tmp;
        // 현재 Container가 비어있으면 특정 위치에 Box를 이동시킨다.
        if (Boxes.Count == 0)
        {
            tmp = gameObject.transform.position;
            tmp.y = -190;
        }
        // Container가 비어있지 않으면, 최상단에 있던 상자 바로 위에 위치시킨다.
        else
        {
            tmp = Boxes[Boxes.Count - 1].transform.position;
            tmp.y += Boxes[Boxes.Count - 1].transform.lossyScale.y;
        }
        // Container의 Box List에 추가한다.
        Boxes.Add(cnt);
        BoxSize++;
        cnt.transform.position = tmp;
        if (Ind == 2 && Boxes.Count == HM.CurBoxNum) HM.ClearEvent();
    }

    // 특정 EventTriggerType에 Event 함수를 종속시킨다.
    // Input : 종속 시킬 Object의 EventTrigger, EventTrigger을 발생시킬 EventType, 종속 시킬 함수

    void AddEvent(EventTrigger eventTrigger, EventTriggerType Type, Action<PointerEventData> Event)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = Type;
        entry.callback.AddListener((data) => { Event((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }
}
