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
    [SerializeField] protected GameObject OpenedProcess;
    [SerializeField] protected bool CanAttatched;
    protected AttatchFile_N AN;
    public string MyName;

    protected override void Awake()
    {
        AN = Dragged.GetComponent<AttatchFile_N>();
        base.Awake();
        image = GetComponent<Image>().sprite;
        CntImage = Dragged.GetComponent<Image>();
        CntRect = Dragged.GetComponent<RectTransform>();
        MyRect = GetComponent<RectTransform>();
        MyUi.AddEvent(GetComponent<EventTrigger>(), EventTriggerType.PointerUp,OpenIcon);
    }

    protected virtual void OpenIcon(PointerEventData Data)
    {
        if (DragDoubleCheck)
        {
            DragDoubleCheck = false;
            return;
        }
        if (Data.clickCount == 2)
        {
            OpenedProcess.SetActive(true);
            ClickEvent();
        }
    }

    protected virtual void ClickEvent()
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
        AN.IsDragged = true;
        CntImage.sprite = image;
        Dragged.gameObject.SetActive(true);
        CntRect.sizeDelta = MyRect.sizeDelta;
    }

    protected override void DragEnd(PointerEventData Data)
    {
        if (AN.IsAttatched)
        {
            if (!CanAttatched) AN.AF.AttatchFail("첨부 실패","제한된 파일 형식");
            else AN.AF.Attatching(image, MyName, gameObject);
        }
        else {
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
        Dragged.gameObject.SetActive(false);
        AN.IsDragged = false;
    }
}
