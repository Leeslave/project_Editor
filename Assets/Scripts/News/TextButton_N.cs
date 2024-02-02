using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextButton_N : Buttons_M
{
    [SerializeField] TextMannager_N TM;
    [SerializeField] bool IsAdd;
    [SerializeField] MainText_N MainText;

    protected override void Click(PointerEventData Data)
    {
        if (IsAdd) TM.ActiveText("Empty",MainText.MyInd);
        else TM.RemoveText(MainText.MyInd);
    }

    protected override void OnPointer(PointerEventData data)
    {
        MainText.OnButton = true;
    }

    protected override void OutPointer(PointerEventData data)
    {
        MainText.OnButton = false;
    }
}
