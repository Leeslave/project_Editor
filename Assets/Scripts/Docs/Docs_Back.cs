using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Docs_Back : Buttons_M
{

    [SerializeField] TextMannager_D TMD;
    [SerializeField] TMP_Text Text;
    [SerializeField] Color AfColor;
    int MyInd;
    RectTransform Rect;
    Outline Out;
    TextMannager_D TD;
    public bool React = true;
    public bool IsSelect = false;

    protected override void Awake()
    {
        base.Awake();
        MyInd = transform.GetSiblingIndex() - 4;
        Rect = transform.parent.GetComponent<RectTransform>();
        Out = GetComponent<Outline>(); 
    }

    Color cnt = new Color(0, 0, 0, 0);
    protected override void Click(PointerEventData Data)
    {
        if (React && !IsSelect)
        {
            image.color = BfColor;
            IsSelect = true;
            TD.Clicked(MyInd, transform);
        }
    }
    protected override void OnPointer(PointerEventData data)
    {
        if (React && !IsSelect)
        {
            image.color = AfColor;
            Out.enabled = true;
        }
    }
    protected override void OutPointer(PointerEventData data)
    {
        if (React && !IsSelect)
        {
            image.color = BfColor;
            Out.enabled = false;
        }
    }

    public void UnSelect()
    {
        image.color = BfColor;
        Out.enabled = false;
        IsSelect = false;
    }

    public void AddTexts(string text, Color color, TextMannager_D pr, TextAlignmentOptions align = TextAlignmentOptions.Left,bool IsTouchAble = true)
    {
        Text.text = text; Text.color = color; Text.alignment = align; if (TD == null) TD = pr;
        image.raycastTarget = IsTouchAble; Out.enabled = false; this.React = IsTouchAble;
        LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
    }

    public void GetCorrected()
    {
        Out.enabled = false; image.raycastTarget = false; Text.fontStyle |= FontStyles.Strikethrough;
    }
}
