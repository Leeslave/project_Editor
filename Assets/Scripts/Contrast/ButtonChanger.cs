using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonChanger : MonoBehaviour
{
    public Color BfColor;
    public Color AfColor;
    private void Start()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        MyUi.ButtonInit(GetComponent<EventTrigger>(), OnPointer, OutPointer, ClickPointer);
    }

    void OnPointer(PointerEventData data)
    {
        GetComponent<Image>().color = AfColor;
    }
    void OutPointer(PointerEventData data)
    {
        GetComponent<Image>().color = BfColor;
    }
    void ClickPointer(PointerEventData data)
    {
        GetComponent<Image>().color = BfColor;
    }
}
