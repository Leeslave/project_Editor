using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Buttons_M : MonoBehaviour
{
    [SerializeField]
    protected InfChange In;
    protected Color BfColor;
    protected TMP_Text text;
    protected Image image;
    private void Awake()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        MyUi.ButtonInit(GetComponent<EventTrigger>(), OnPointer, OutPointer, Click);
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
