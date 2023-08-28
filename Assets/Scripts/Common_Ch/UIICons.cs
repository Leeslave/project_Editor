using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIICons : UIDragger
{
    protected Image MyImage;
    protected RectTransform ImageRect;
    protected TMP_Text MyText;
    protected Image CntImage;
    protected RectTransform CntRect;
    protected RectTransform MyRect;
    protected bool DragDoubleCheck = false;
    // LayOut에 맞춰서 배치되는지 여부
    [SerializeField] protected bool IsLayer = true;
    // Icon을 통해 실행되는 프로세스
    [SerializeField] protected GameObject OpenedProcess;
    // 메세지에 첨부될 수 있는 파일인지
    [SerializeField] protected bool CanAttatched;
    // WindowsManager
    [SerializeField] protected Windows_M WM;
    protected AttatchFile_N AN;
    protected Tuple<int, int> CurLay;
    public int PoolNum;

    protected override void Awake()
    {
        AN = Dragged.GetComponent<AttatchFile_N>();
        base.Awake();
        ImageRect = transform.GetChild(0).GetComponent<RectTransform>();
        MyImage = transform.GetChild(0).GetComponent<Image>();
        MyText = transform.GetChild(1).GetComponent<TMP_Text>();
        CntImage = Dragged.GetComponent<Image>();
        CntRect = Dragged.GetComponent<RectTransform>();
        MyRect = GetComponent<RectTransform>();
        MyUi.AddEvent(GetComponent<EventTrigger>(), EventTriggerType.PointerUp,OpenIcon);
    }

    protected virtual void OnEnable()
    {
        CurLay = WM.BatchByCreate(gameObject);
        print(name);
    }
    protected virtual void OnDisable()
    {
        WM.RemoveIcon(CurLay);
    }
    public virtual void ClearIcon()
    {
        WM.ClearIcon(PoolNum);
    }

    public virtual void Init(bool AttatchAble, GameObject OpenProcess, string name, Sprite Image, int num)
    {
        OpenedProcess = OpenProcess;
        CanAttatched = AttatchAble;
        MyText.text = name;
        MyImage.sprite = Image;
        PoolNum = num;
    }

    protected virtual void OpenIcon(PointerEventData Data)
    {
        if(OpenedProcess != null)
            if (OpenedProcess.activeSelf) return;
        if (DragDoubleCheck)
        {
            DragDoubleCheck = false;
            return;
        }
        if (Data.clickCount == 2)
        {
            if(OpenedProcess!=null)OpenedProcess.SetActive(true);
            ClickEvent();
        }
    }

    protected virtual void ClickEvent()
    {

    }

    protected override void Click(PointerEventData Data)
    {
        if (OpenedProcess != null)
            if (OpenedProcess.activeSelf) return;
        base.Click(Data);
        CntRect.position = MyRect.position;
    }

    protected override void DragOn(PointerEventData Data)
    {
        if (OpenedProcess != null)
            if (OpenedProcess.activeSelf) return;
        base.DragOn(Data);
        if (Data.clickCount == 2) DragDoubleCheck = true;
        Dragged.gameObject.SetActive(true);
        AN.IsDragged = true;
        CntImage.sprite = MyImage.sprite;
        AN.CurDragged = gameObject;
        CntRect.sizeDelta = ImageRect.sizeDelta;
    }

    protected override void DragEnd(PointerEventData Data)
    {
        if (OpenedProcess != null)
            if (OpenedProcess.activeSelf) return;
        if (AN.IsAttatched)
        {
            if (!CanAttatched) AN.AF.AttatchFail("첨부 실패","제한된 파일 형식");
            else AN.AF.Attatching(MyImage.sprite, MyText.text, gameObject);
        }
        else if (AN.IsDumped)
        {
            AN.DI.DumpAdd(gameObject,MyImage.sprite,MyText.text);
            gameObject.SetActive(false);
        }
        else {
            if (IsLayer) WM.BatchByMove(gameObject, Dragged, ref CurLay);
            else MyRect.position = CntRect.position;
        }
        Dragged.gameObject.SetActive(false);
    }
}
