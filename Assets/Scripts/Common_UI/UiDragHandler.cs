using UnityEngine;
using UnityEngine.EventSystems;

public class UiDragHandler : MonoBehaviour
{
    [SerializeField]
    protected Transform _dragTarget;
    protected Vector3 _pointerOffset;
    protected EventTrigger _eventTrigger;

    protected virtual void Awake()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        _eventTrigger = GetComponent<EventTrigger>();
        UiEventUtility.AddEvent(_eventTrigger, EventTriggerType.PointerDown, Click);
        UiEventUtility.AddEvent(_eventTrigger, EventTriggerType.BeginDrag,DragOn);
        UiEventUtility.AddEvent(_eventTrigger, EventTriggerType.EndDrag, DragEnd);
        UiEventUtility.AddEvent(_eventTrigger, EventTriggerType.Drag, DragPointer);
    }

    /// <summary>
    /// 드래그 중인 Object를 최상단에 위치시킴.
    /// </summary>
    /// <param name="Data"></param>
    protected virtual void Click(PointerEventData data)
    {
        _dragTarget.SetAsLastSibling();
    }

    protected virtual void DragOn(PointerEventData data)
    {
        DragSetting();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="data"></param>
    protected virtual void DragPointer(PointerEventData data)
    {
        UiEventUtility.Drag(_dragTarget.gameObject, _pointerOffset);
    }

    /// <summary>
    /// 마우스 좌표를 UI용 좌표로 변환(무조건 Canvas가 World세팅이 되어 있어야 함)
    /// </summary>
    protected virtual void DragSetting()
    {
        _pointerOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _dragTarget.position;
        _pointerOffset.z = 0;
    }
    protected virtual void DragEnd(PointerEventData data)
    {

    }
}
