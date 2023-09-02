using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Buttons_Obs : MonoBehaviour
{
    /// <summary>
    /// 포인터 온, 오프, 클릭 이벤트만 초기화되있음.
    /// </summary>
    protected virtual void Awake()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        MyUi.ButtonInit(GetComponent<EventTrigger>(), OnPointer, OutPointer, Click);
    }

    protected virtual void OnPointer(PointerEventData data) { }
    protected virtual void OutPointer(PointerEventData data) { }
    protected virtual void Click(PointerEventData Data) { }
}
