using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Docs_Back : Buttons_M
{

    [FormerlySerializedAs("TMD")]
    [SerializeField] private TextMannager_D defaultManager;
    [FormerlySerializedAs("Text")]
    [SerializeField] private TMP_Text textLabel;
    [FormerlySerializedAs("AfColor")]
    [SerializeField] private Color hoverColor;
    private int entryIndex;
    private RectTransform parentRect;
    private Outline outline;
    private TextMannager_D manager;
    public bool React = true;
    public bool IsSelect = false;

    protected override void Awake()
    {
        base.Awake();
        entryIndex = transform.GetSiblingIndex() - 4;
        parentRect = transform.parent.GetComponent<RectTransform>();
        outline = GetComponent<Outline>();
    }

    protected override void Click(PointerEventData eventData)
    {
        if (React && !IsSelect)
        {
            image.color = BfColor;
            IsSelect = true;
            manager.Clicked(entryIndex, transform);
        }
    }
    protected override void OnPointer(PointerEventData data)
    {
        if (React && !IsSelect)
        {
            image.color = hoverColor;
            outline.enabled = true;
        }
    }
    protected override void OutPointer(PointerEventData data)
    {
        if (React && !IsSelect)
        {
            image.color = BfColor;
            outline.enabled = false;
        }
    }

    public void UnSelect()
    {
        image.color = BfColor;
        outline.enabled = false;
        IsSelect = false;
    }

    public void AddTexts(string text, Color color, TextMannager_D pr, TextAlignmentOptions align = TextAlignmentOptions.Left,bool IsTouchAble = true)
    {
        textLabel.text = text;
        textLabel.color = color;
        textLabel.alignment = align;
        if (manager == null)
            manager = pr != null ? pr : defaultManager;
        image.raycastTarget = IsTouchAble;
        outline.enabled = false;
        React = IsTouchAble;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
    }

    public void GetCorrected()
    {
        outline.enabled = false;
        image.raycastTarget = false;
        textLabel.fontStyle |= FontStyles.Strikethrough;
    }
}
