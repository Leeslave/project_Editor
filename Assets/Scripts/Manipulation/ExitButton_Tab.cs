using UnityEngine;
using UnityEngine.EventSystems;

public class ExitButton_Tab : Buttons_M
{
    public TabManager_M TM;
    [SerializeField]
    int TabNum;

    protected override void OnPointer(PointerEventData Data)
    {
        if (In.IsTouchAble()) text.color = Color.red;
    }
    protected override void OutPointer(PointerEventData Data)
    {
        if (In.IsTouchAble()) text.color = BfColor;
    }
    protected override void Click(PointerEventData Data)
    {
        text.color = BfColor;
        TM.DeleteTab(TabNum);
    }
}
