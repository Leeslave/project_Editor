using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonChanger : MonoBehaviour
{
    bool highlighted = false;
    public Color BfColor;
    public Color AfColor;
    private void Start()
    {
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
        entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_PointerEnter.callback.AddListener((data) => { OnPointer((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerEnter);

        EventTrigger.Entry entry_PointerOut = new EventTrigger.Entry();
        entry_PointerOut.eventID = EventTriggerType.PointerExit;
        entry_PointerOut.callback.AddListener((data) => { OutPointer((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerOut);
    }

    void OnPointer(PointerEventData data)
    {
        GetComponent<Image>().color = AfColor;
    }
    void OutPointer(PointerEventData data)
    {
        GetComponent<Image>().color = BfColor;
    }
}
