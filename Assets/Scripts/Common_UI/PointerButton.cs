using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class PointerButton : MonoBehaviour
{
    [SerializeField]
    protected Color _defaultColor;
    protected TMP_Text _textLabel;
    protected Image _buttonImage;
    protected EventTrigger _eventTrigger;
    /// <summary>
    /// 포인터 온, 오프, 클릭 이벤트만 초기화되있음.
    /// </summary>
    protected virtual void Awake()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        _eventTrigger = GetComponent<EventTrigger>();
        UiEventUtility.InitializeButton(_eventTrigger, OnPointer, OutPointer, Click);
        if (TryGetComponent(out TMP_Text label))
        {
            _textLabel = label;
            _defaultColor = _textLabel.color;
        }
        else
        {
            _buttonImage = GetComponent<Image>();
            if (_buttonImage != null) _defaultColor = _buttonImage.color;
        }
    }

    protected virtual void OnPointer(PointerEventData data) { }
    protected virtual void OutPointer(PointerEventData data) { }
    protected virtual void Click(PointerEventData data) { }
}
