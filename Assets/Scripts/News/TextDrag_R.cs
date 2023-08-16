using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextDrag_R : UIDragger
{
    [SerializeField] TextMannager_N TM;
    [SerializeField] TMP_Text ChildText;
    [SerializeField] int MyInd;
    RectTransform MySize;
    Outline OL;

    protected override void Awake()
    {
        base.Awake();
        OL = GetComponent<Outline>();
        MySize = GetComponent<RectTransform>();
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerEnter, OnPoint);
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerExit, OutPoint);
    }
    protected override void Click(PointerEventData Data)
    {
        base.Click(Data);
        TM.CntSize.position = MySize.position;
    }

    protected override void DragOn(PointerEventData Data)
    {
        base.DragOn(Data);
        OL.effectColor = TM.ColorN;
        TM.IsDragged = true;
        TM.CntText.text = ChildText.text;
        TM.CntSize.sizeDelta = MySize.sizeDelta;
        Dragged.gameObject.SetActive(true);
    }
    protected override void DragEnd(PointerEventData Data)
    {
        TM.IsDragged = false;
        Dragged.gameObject.SetActive(false);
        if (TM.EndDrag(MyInd)) gameObject.SetActive(false);
    }

    void OnPoint(PointerEventData Data)
    {
        if(!TM.IsDragged)OL.effectColor = TM.ColorT;
    }

    void OutPoint(PointerEventData Data)
    {
        if(!TM.IsDragged)OL.effectColor = TM.ColorN;
    }

}
