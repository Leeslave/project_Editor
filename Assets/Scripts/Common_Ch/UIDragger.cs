using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragger : MonoBehaviour
{
    [SerializeField]
    protected Transform Dragged;
    protected Vector3 AnchorGap;
    protected EventTrigger eventTrigger;

    protected virtual void Awake()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        eventTrigger = GetComponent<EventTrigger>();
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerDown, Click);
        MyUi.AddEvent(eventTrigger, EventTriggerType.BeginDrag,DragOn);
        MyUi.AddEvent(GetComponent<EventTrigger>(), EventTriggerType.EndDrag, DragEnd);
        MyUi.AddEvent(eventTrigger, EventTriggerType.Drag, DragPointer);
    }

    protected virtual void Click(PointerEventData Data)
    {
        Dragged.SetAsLastSibling();
    }

    protected virtual void DragOn(PointerEventData Data)
    {
        DragSetting();
    }

    protected virtual void DragPointer(PointerEventData data)
    {
        MyUi.DragUI(Dragged.gameObject, AnchorGap);
    }

    protected virtual void DragSetting() 
    { 
        AnchorGap = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Dragged.position; 
        AnchorGap.z = 0;
    }
    protected virtual void DragEnd(PointerEventData Data)
    {

    }
}
