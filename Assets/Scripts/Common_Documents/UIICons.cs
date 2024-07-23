using System;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIICons : UIDragger
{
    /*
     * Icon의 경우 Icon Object 밑에 Image 부분, Text 부분으로 구성되어 있다.
     */

    // Icon의 image 부분
    protected Image MyImage;
    // Icon의 Image 부분의 Transform
    protected RectTransform ImageRect;
    // Icon의 Text
    protected TMP_Text MyText;
    // Icon.의 Text 부분의 Transform
    protected RectTransform TextRect;
    // 드래그 시 사용될 Image
    protected Image CntImage;
    // 드래그 시 이동되는 임시 Object의 Transform
    protected RectTransform CntRect;
    // Icon 자체 Object의 Transform
    protected RectTransform MyRect;
    // Drag시 DoubleCheck에 카운터 되는 것을 방지하기 위해 사용
    protected bool DragDoubleCheck = false;
    // LayOut에 맞춰서 배치되는지 여부
    [SerializeField] protected bool IsLayer = true;
    // Icon을 통해 실행되는 프로세스
    [SerializeField] protected GameObject OpenedProcess;
    // 0 : Folder, 1 : DB_M.DB_Docs, 2 : Text, 3 : 몰라
    public int type;
    // WindowsManager.cs
    [NonSerialized] public Windows_M Window;
    // Window상에서 배치되어 있는 LayOut의 위치 정보 저장에 사용.
    protected Tuple<int, int> CurLay = null;
    // Pooling용.
    public int PoolNum;

    protected bool IsAwakened = true;

    protected override void Awake()
    {
        base.Awake();
        ImageRect = transform.GetChild(0).GetComponent<RectTransform>();
        MyImage = transform.GetChild(0).GetComponent<Image>();
        MyText = transform.GetChild(1).GetComponent<TMP_Text>();
        TextRect = MyText.GetComponent<RectTransform>();
        CntImage = Dragged.GetComponent<Image>();
        CntRect = Dragged.GetComponent<RectTransform>();
        MyRect = GetComponent<RectTransform>();
        MyUi.AddEvent(GetComponent<EventTrigger>(), EventTriggerType.PointerUp,OpenIcon);
    }
    protected virtual void Start()
    {
        CurLay = Window.BatchByCreate(gameObject);
        ImageRect.sizeDelta = MyRect.sizeDelta * 0.8f;
        float cnt = ImageRect.sizeDelta.x - ImageRect.sizeDelta.y;
        ImageRect.anchoredPosition = new Vector2(0,(ImageRect.sizeDelta.x - ImageRect.sizeDelta.y)*0.5f);
        TextRect.anchoredPosition = new Vector2(0, -(ImageRect.sizeDelta.y) * 0.5f - 10);
        MyText.fontSize = cnt;
        IsAwakened = false;
    }
    protected virtual void OnEnable()
    {
        if(!IsAwakened)CurLay = Window.BatchByCreate(gameObject);
    }
    protected virtual void OnDisable()
    {
        if(CurLay!=null)Window.RemoveIcon(CurLay);
    }
    public virtual void ClearIcon()
    {
        Window.ClearIcon(PoolNum);
        OpenedProcess = null;
        CurLay = null;
    }

    /// <summary>
    /// Icon을 초기화
    /// </summary>
    /// <param name="AttatchAble">첨부 가능 여부</param>
    /// <param name="OpenProcess">클릭을 통해 실행 될 Process</param>
    /// <param name="name">Icon의 이름(Text에 사용)</param>
    /// <param name="Image">Icon의 이미지(Image에 사용)</param>
    /// <param name="num">PoolingNumber</param>
    public virtual void Init(bool AttatchAble, GameObject OpenProcess, string name, Sprite Image, int num,int type)
    {
        this.name = name;
        OpenedProcess = OpenProcess;
        MyText.text = name;
        MyImage.sprite = Image;
        PoolNum = num;
        this.type = type;
    }
    /// <summary>
    /// Icon 더블 클릭 탐지.
    /// 탐지 시 해당 Icon의 프로그램 실행.
    /// </summary>
    /// <param name="Data"></param>
    protected virtual void OpenIcon(PointerEventData Data)
    {
        if (OpenedProcess != null)
        {
            if (OpenedProcess.activeSelf) return;
        }
        if (DragDoubleCheck)
        {
            DragDoubleCheck = false;
            return;
        }
        if (Data.clickCount == 2)
        {
            if (OpenedProcess != null)
            {
                OpenedProcess.SetActive(true);
                OpenedProcess.transform.SetAsLastSibling();
            }
            ClickEvent();
        }
    }
    /// <summary>
    /// Icon 더블 클릭시 발생하는 이벤트.
    /// </summary>
    protected virtual void ClickEvent()
    {

    }
    
    protected override void Click(PointerEventData Data)
    {
        base.Click(Data);
        CntRect.position = MyRect.position;
    }

    /// <summary>
    /// Drag시 임시 Object를 활성화하며, 임시 Object의 Image를 Icon의 Image로 변환
    /// </summary>
    /// <param name="Data"></param>
    protected override void DragOn(PointerEventData Data)
    {
        base.DragOn(Data);
        if (Data.clickCount == 2) DragDoubleCheck = true;
        Dragged.gameObject.SetActive(true);
        DB_M.DB_Docs.CntFileForAttach.IsDragged = true;
        DB_M.DB_Docs.CntFileForAttach.AttatchType = type;
        DB_M.DB_Docs.CntFileForAttach.IconName = name;
        CntImage.sprite = MyImage.sprite;
        DB_M.DB_Docs.CntFileForAttach.CurDragged = gameObject;
        CntRect.sizeDelta = ImageRect.sizeDelta;
        
    }

    /// <summary>
    /// 드래그 종료 시 Icon의 첨부, 이동, 삭제를 구분.
    /// </summary>
    /// <param name="Data"></param>
    protected override void DragEnd(PointerEventData Data)
    {
        if (DB_M.DB_Docs.CntFileForAttach.Attatch!=null) DB_M.DB_Docs.CntFileForAttach.Attatch();
        else if (IsLayer) Window.BatchByMove(gameObject, Dragged, ref CurLay);
        else MyRect.position = CntRect.position;
        Dragged.gameObject.SetActive(false);
    }
}
