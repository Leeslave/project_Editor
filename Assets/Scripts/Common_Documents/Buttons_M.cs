using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Buttons_M : MonoBehaviour
{
    [SerializeField]
    protected Color BfColor;
    protected TMP_Text text;
    protected Image image;
    protected EventTrigger ET;
    /// <summary>
    /// 포인터 온, 오프, 클릭 이벤트만 초기화되있음.
    /// </summary>
    protected virtual void Awake()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        ET = GetComponent<EventTrigger>();
        MyUi.ButtonInit(ET, OnPointer, OutPointer, Click);
        if (TryGetComponent(out TMP_Text Text))
        {
            text = Text;
            BfColor = text.color;
        }
        else
        {
            image = GetComponent<Image>();
            BfColor = image.color;
        }
    }

    protected virtual void OnPointer(PointerEventData data) { }
    protected virtual void OutPointer(PointerEventData data) { }
    protected virtual void Click(PointerEventData Data) { }
}
