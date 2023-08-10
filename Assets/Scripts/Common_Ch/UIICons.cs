using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIICons : Dragger_CM
{
    protected Sprite image;
    protected Image CntImage;
    protected RectTransform CntRect;
    protected RectTransform MyRect;

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
        if(Data.clickCount == 2)
        {
            print("!");
        }
    }

    protected override void Click(PointerEventData Data)
    {
        base.Click(Data);
        CntRect.position = MyRect.position;
    }

    protected override void DragOn(PointerEventData Data)
    {
        base.DragOn(Data);
        CntImage.sprite = image;
        Dragged.gameObject.SetActive(true);
        CntRect.sizeDelta = MyRect.sizeDelta;
    }

    void DragEnd(PointerEventData Data)
    {
        MyRect.position = CntRect.position;
        Dragged.gameObject.SetActive(false);
    }


}
