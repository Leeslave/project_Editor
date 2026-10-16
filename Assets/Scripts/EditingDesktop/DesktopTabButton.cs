using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopTabButton : PointerButton
{
    public List<GameObject> Subs;
    [SerializeField] private int _myIndex;
    public bool IsOpened;

    [SerializeField] private Sprite _nonSelect;
    [SerializeField] private Sprite _select;

    protected override void OnPointer(PointerEventData data)
    {
        if (IsOpened) return;
        _buttonImage.sprite = _select;
    }
    protected override void OutPointer(PointerEventData data)
    {
        if (IsOpened) return;
        _buttonImage.sprite = _nonSelect;
    }
    protected override void Click(PointerEventData data)
    {
        if (IsOpened) return;
        OpenTab();
    }

    public void OpenTab()
    {
        foreach (GameObject s in Subs) s.SetActive(true);
        _buttonImage.sprite = _select;
        EditingDesktopController.Instance.FileViewer.ChangeTab(_myIndex);
        IsOpened = true;
    }

    public void CloseTab()
    {
        IsOpened = false;
        if (_buttonImage != null) _buttonImage.sprite = _nonSelect;
        foreach (GameObject s in Subs) s.SetActive(false);
    }
}
