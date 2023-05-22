using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForDrag_M : MonoBehaviour
{
    public InfChange In;
    Transform Parent;
    Vector3 AnchorGap;

    private void Start()
    {
        Parent = transform.parent;
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerDown, Click);
        MyUi.AddEvent(eventTrigger, EventTriggerType.BeginDrag, DragOn);
        MyUi.AddEvent(eventTrigger, EventTriggerType.Drag, DragPointer);
    }

    void Click(PointerEventData Data)
    {
        if(In.IsTouchAble()) Parent.SetAsLastSibling();
    }

    void DragOn(PointerEventData Data)
    {
        if (In.IsTouchAble()) DragSetting();
    }

    void DragPointer(PointerEventData data)
    {
        if (In.IsTouchAble()) MyUi.DragUI(Parent.gameObject, AnchorGap);
    }

    void DragSetting()
    {
        AnchorGap = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.parent.position;
        AnchorGap.z = 0;
    }
}
