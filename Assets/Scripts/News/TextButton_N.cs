using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextButton_N : Buttons_M
{
    [SerializeField] MainText_N MainText;

    protected override void OnPointer(PointerEventData data)
    {
        MainText.OnButton = true;
    }

    protected override void OutPointer(PointerEventData data)
    {
        MainText.OnButton = false;
    }
}
