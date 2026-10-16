using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UiEventUtility
{
    public static void Drag(GameObject target, Vector3 pointerOffset)
    {
        Camera camera = Camera.main;
        if (camera == null) return;
        Vector3 position = camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        target.transform.position = position - pointerOffset;
    }

    public static void AddEvent(EventTrigger trigger, EventTriggerType type, Action<PointerEventData> callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(data => callback((PointerEventData)data));
        trigger.triggers.Add(entry);
    }

    public static void InitializeButton(EventTrigger trigger, Action<PointerEventData> onEnter,
        Action<PointerEventData> onExit, Action<PointerEventData> onClick)
    {
        if (onEnter != null) AddEvent(trigger, EventTriggerType.PointerEnter, onEnter);
        if (onExit != null) AddEvent(trigger, EventTriggerType.PointerExit, onExit);
        if (onClick != null) AddEvent(trigger, EventTriggerType.PointerClick, onClick);
    }
}
