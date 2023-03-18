using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MyUi
{
     public static Vector3 UIPosition(GameObject a) { return a.GetComponent<RectTransform>().anchoredPosition; }
     public static Vector3 UISize(GameObject a) { return a.GetComponent<RectTransform>().sizeDelta; }
     public static void ChangeUIPosition(ref GameObject a, Vector3 l) { a.GetComponent<RectTransform>().anchoredPosition = l; }
     public static GameObject GRay(GraphicRaycaster gr)
    {
        var ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        if (gr == null) return null;
        gr.Raycast(ped, results);

        if (results.Count <= 0) return null;
        return results[0].gameObject;
    }
    public static void DragUI(GameObject DragingObject, Vector3 AnchorGap) { Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); mousePos.z = 0; DragingObject.transform.position = mousePos - AnchorGap; }
    public static int StringToInt(string cnt) { int a = 0; foreach (char b in cnt) a = a * 10 + (b - '0'); return a; }

    public static void ButtonInit(EventTrigger eventTrigger, Action<PointerEventData> OnPointer, Action<PointerEventData> OutPointer, Action<PointerEventData> ClickPointer)
    {
        //data.pointerId : left -> -1, right -> -2, Wheel -> -3;
        if (OnPointer != null)
        {
            EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
            entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
            entry_PointerEnter.callback.AddListener((data) => { OnPointer((PointerEventData)data); });
            eventTrigger.triggers.Add(entry_PointerEnter);
        }

        if (OutPointer != null)
        {
            EventTrigger.Entry entry_PointerOut = new EventTrigger.Entry();
            entry_PointerOut.eventID = EventTriggerType.PointerExit;
            entry_PointerOut.callback.AddListener((data) => { OutPointer((PointerEventData)data); });
            eventTrigger.triggers.Add(entry_PointerOut);
        }

        if (ClickPointer != null)
        {
            EventTrigger.Entry entry_PointerClick = new EventTrigger.Entry();
            entry_PointerClick.eventID = EventTriggerType.PointerClick;
            entry_PointerClick.callback.AddListener((data) => { ClickPointer((PointerEventData)data); });
            eventTrigger.triggers.Add(entry_PointerClick);
        }
    }
}
