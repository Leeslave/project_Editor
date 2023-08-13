using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIICons : UIDragger
{
    protected Sprite image;
    protected Image CntImage;
    protected RectTransform CntRect;
    protected RectTransform MyRect;
    protected bool DragDoubleCheck = false;
    [SerializeField] protected bool IsLayer = true;

    protected override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>().sprite;
        CntImage = Dragged.GetComponent<Image>();
        CntRect = Dragged.GetComponent<RectTransform>();
        MyRect = GetComponent<RectTransform>();
        MyUi.AddEvent(GetComponent<EventTrigger>(), EventTriggerType.EndDrag, DragEnd);
        MyUi.AddEvent(GetComponent<EventTrigger>(), EventTriggerType.PointerUp,ClickEvent);
    }

    protected virtual void ClickEvent(PointerEventData Data)
    {
    }

    protected override void Click(PointerEventData Data)
    {
        base.Click(Data);
        CntRect.position = MyRect.position;
    }

    protected override void DragOn(PointerEventData Data)
    {
        base.DragOn(Data);
        if (Data.clickCount == 2) DragDoubleCheck = true;
        CntImage.sprite = image;
        Dragged.gameObject.SetActive(true);
        CntRect.sizeDelta = MyRect.sizeDelta;
    }

    void DragEnd(PointerEventData Data)
    {
        Dragged.gameObject.SetActive(false);
        if (IsLayer)
        {
            float x = CntRect.position.x;
            if (x <= -400) x = -500;
            else if (x <= -200) x = -300;
            else if (x <= 0) x = -100;
            else if (x <= 200) x = 100;
            else if (x <= 400) x = 300;
            else x = 500;
            float y = CntRect.position.y;
            if (y <= -320) y = -400;
            else if (y <= -160) y = -240;
            else if (y <= 0) y = -80;
            else if (y <= 160) y = 80;
            else if (y <= 320) y = 240;
            else y = 400;
            MyRect.position = new Vector3(x, y, 0);
        }
        else MyRect.position = CntRect.position;
    }


}
