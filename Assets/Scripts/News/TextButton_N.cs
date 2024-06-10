using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 줄 생성, 삭제 버튼
public class TextButton_N : Buttons_M
{
    [SerializeField] UnityEngine.UI.Image MyImage;
    [SerializeField] MainText_N MainText;
    [SerializeField] bool IsRemove;
    [SerializeField] Color AfColor;

    protected override void Click(PointerEventData Data)
    {
        MyImage.color = BfColor;
    }

    protected override void OnPointer(PointerEventData data)
    {
        MyImage.color = AfColor;
        if (IsRemove) MainText.OnRemoveButton = true;
        else MainText.OnAddButton = true;
    }

    protected override void OutPointer(PointerEventData data)
    {
        MyImage.color = BfColor;
        if (IsRemove) MainText.OnRemoveButton = false;
        else MainText.OnAddButton = false;
    }

    private void OnDisable()
    {
        MyImage.color = BfColor;
    }
}
