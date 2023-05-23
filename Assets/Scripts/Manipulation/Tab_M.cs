using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tab_M : Buttons_M
{
    public TabManager_M TM;
    HighLighter_M CurFolder = null;
    Color AfColor;
    Color SelectColor;
    [SerializeField]
    bool IsSelected;
    [SerializeField]
    int TabNum;

    private void Start()
    {
        AfColor = BfColor + new Color(0.1f, 0.1f, 0.1f, 0);
        SelectColor = AfColor + new Color(0.2f, 0.2f, 0.2f, 0);
        if (TabNum == 0)
        {
            image.color = SelectColor;
            IsSelected = true;
        }
    }

    protected override void OnPointer(PointerEventData Data)
    {
        if (In.IsTouchAble() && !IsSelected) image.color = AfColor;
    }
    protected override void OutPointer(PointerEventData Data)
    {
        if (In.IsTouchAble() && !IsSelected) image.color = BfColor;
    }
    protected override void Click(PointerEventData Data)
    {
        if (!In.IsTouchAble() && IsSelected) return;
        image.color = SelectColor;
        IsSelected = true;
        TM.ChangeTab(TabNum);
        In.CloseFolder();
        In.OpenFolder(CurFolder);
    }
    private void OnEnable()
    {
        CurFolder = null;
        IsSelected = false;
    }
    public void ChangeFolder(HighLighter_M cnt)
    {
        CurFolder = cnt;
    }
    public void Deselect()
    {
        image.color = BfColor;
        IsSelected = false;
    }
}
