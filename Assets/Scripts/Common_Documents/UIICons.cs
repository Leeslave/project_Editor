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
     * Icon�� ��� Icon Object �ؿ� Image �κ�, Text �κ����� �����Ǿ� �ִ�.
     */

    // Icon�� image �κ�
    protected Image MyImage;
    // Icon�� Image �κ��� Transform
    protected RectTransform ImageRect;
    // Icon�� Text
    protected TMP_Text MyText;
    // Icon.�� Text �κ��� Transform
    protected RectTransform TextRect;
    // �巡�� �� ���� Image
    protected Image CntImage;
    // �巡�� �� �̵��Ǵ� �ӽ� Object�� Transform
    protected RectTransform CntRect;
    // Icon ��ü Object�� Transform
    protected RectTransform MyRect;
    // Drag�� DoubleCheck�� ī���� �Ǵ� ���� �����ϱ� ���� ���
    protected bool DragDoubleCheck = false;
    // LayOut�� ���缭 ��ġ�Ǵ��� ����
    [SerializeField] protected bool IsLayer = true;
    // Icon�� ���� ����Ǵ� ���μ���
    [SerializeField] protected GameObject OpenedProcess;
    // 0 : Folder, 1 : DB, 2 : Text, 3 : ����
    public int type;
    // WindowsManager.cs
    [NonSerialized] public Windows_M WM;
    protected AttatchFile_N AN;
    // Window�󿡼� ��ġ�Ǿ� �ִ� LayOut�� ��ġ ���� ���忡 ���.
    protected Tuple<int, int> CurLay = null;
    // Pooling��.
    public int PoolNum;

    protected bool IsAwakened = true;

    protected override void Awake()
    {
        AN = Dragged.GetComponent<AttatchFile_N>();
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
        CurLay = WM.BatchByCreate(gameObject);
        ImageRect.sizeDelta = MyRect.sizeDelta * 0.8f;
        float cnt = ImageRect.sizeDelta.x - ImageRect.sizeDelta.y;
        ImageRect.anchoredPosition = new Vector2(0,(ImageRect.sizeDelta.x - ImageRect.sizeDelta.y)*0.5f);
        TextRect.anchoredPosition = new Vector2(0, -(ImageRect.sizeDelta.y) * 0.5f - 10);
        MyText.fontSize = cnt;
        IsAwakened = false;
    }
    protected virtual void OnEnable()
    {
        if(!IsAwakened)CurLay = WM.BatchByCreate(gameObject);
    }
    protected virtual void OnDisable()
    {
        if(CurLay!=null)WM.RemoveIcon(CurLay);
    }
    public virtual void ClearIcon()
    {
        WM.ClearIcon(PoolNum);
        OpenedProcess = null;
        CurLay = null;
    }

    /// <summary>
    /// Icon�� �ʱ�ȭ
    /// </summary>
    /// <param name="AttatchAble">÷�� ���� ����</param>
    /// <param name="OpenProcess">Ŭ���� ���� ���� �� Process</param>
    /// <param name="name">Icon�� �̸�(Text�� ���)</param>
    /// <param name="Image">Icon�� �̹���(Image�� ���)</param>
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
    /// Icon ���� Ŭ�� Ž��.
    /// Ž�� �� �ش� Icon�� ���α׷� ����.
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
    /// Icon ���� Ŭ���� �߻��ϴ� �̺�Ʈ.
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
    /// Drag�� �ӽ� Object�� Ȱ��ȭ�ϸ�, �ӽ� Object�� Image�� Icon�� Image�� ��ȯ
    /// </summary>
    /// <param name="Data"></param>
    protected override void DragOn(PointerEventData Data)
    {
        base.DragOn(Data);
        if (Data.clickCount == 2) DragDoubleCheck = true;
        Dragged.gameObject.SetActive(true);
        AN.IsDragged = true;
        AN.AttatchType = type;
        AN.IconName = name;
        CntImage.sprite = MyImage.sprite;
        AN.CurDragged = gameObject;
        CntRect.sizeDelta = ImageRect.sizeDelta;
    }

    /// <summary>
    /// �巡�� ���� �� Icon�� ÷��, �̵�, ������ ����.
    /// </summary>
    /// <param name="Data"></param>
    protected override void DragEnd(PointerEventData Data)
    {
        if (AN.Attatch!=null) AN.Attatch();
        else if (IsLayer) WM.BatchByMove(gameObject, Dragged, ref CurLay);
        else MyRect.position = CntRect.position;
        Dragged.gameObject.SetActive(false);
    }
}
