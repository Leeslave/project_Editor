using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabAdder_M : Buttons_M
{
    public TabManager_M TM;

    
    Color AfColor;
    private void Start()
    {
        AfColor = BfColor + new Color(0.1f, 0.1f, 0.1f, 0);
    }

    protected override void OnPointer(PointerEventData Data)
    {
        if (In.IsTouchAble()) image.color = AfColor;
    }
    protected override void OutPointer(PointerEventData Data)
    {
        if (In.IsTouchAble()) image.color = BfColor;
    }
    protected override void Click(PointerEventData Data)
    {
        if (!In.IsTouchAble()) return;
        image.color += BfColor;
        if (TM.AddTab()) gameObject.SetActive(false);
    }
}
