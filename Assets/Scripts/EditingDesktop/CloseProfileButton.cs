using UnityEngine;
using UnityEngine.EventSystems;

public class CloseProfileButton : PointerButton
{
    [SerializeField] private InfChange _profileEditor;
    private Color _hoverColor;
    // 포인터 강조에 사용할 버튼 색을 준비한다.
    private void Start()
    {
        _hoverColor = _defaultColor + new Color(0.1f, 0.1f, 0.1f, 0);
    }

    protected override void OnPointer(PointerEventData data)
    {
        if (_profileEditor.IsTouchAble()) _buttonImage.color = _hoverColor;
    }
    protected override void OutPointer(PointerEventData data)
    {
        if (_profileEditor.IsTouchAble()) _buttonImage.color = _defaultColor;
    }
    protected override void Click(PointerEventData data)
    {
        if (!_profileEditor.IsTouchAble()) return;
        _buttonImage.color = _defaultColor;
        _profileEditor.CloseFolder();
    }
}
