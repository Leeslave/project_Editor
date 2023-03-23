using UnityEngine;
using UnityEngine.EventSystems;

public class ForDrag : MonoBehaviour
{
    public GameObject contrastmanager;
    ContrastManager CM;

    Transform Parent;
    Vector3 AnchorGap;

    private void Start()
    {
        CM = contrastmanager.GetComponent<ContrastManager>();

        Parent = transform.parent;
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerDown, Click);
        MyUi.AddEvent(eventTrigger, EventTriggerType.BeginDrag,DragOn);
        MyUi.AddEvent(eventTrigger, EventTriggerType.Drag, DragPointer);
    }

    void Click(PointerEventData Data)
    {
        if (CM.JudgeTime) return;
        Parent.SetAsLastSibling();
        CM.InputCall();
    }

    void DragOn(PointerEventData Data)
    {
        if (!CM.JudgeTime)DragSetting();
    }

    void DragPointer(PointerEventData data)
    {
        if(!CM.JudgeTime)MyUi.DragUI(Parent.gameObject, AnchorGap);
    }

    void DragSetting() 
    { 
        AnchorGap = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.parent.position; 
        AnchorGap.z = 0;
    }
}
