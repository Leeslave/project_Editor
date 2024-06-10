using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tabs_D : Buttons_M
{
    [SerializeField] GetOptionFile_D GOF;
    public List<GameObject> Subs;
    [SerializeField] int MyIndex;
    public bool IsOpened;

    Color Sub = new Color(0.1f, 0.1f, 0.1f);
    Color OpenColor = new Color(0.254902f, 0.254902f, 0.254902f);
    Color CloseColor = new Color(0.1215686f, 0.1215686f, 0.1215686f);
    protected override void Awake()
    {
        base.Awake();
        BfColor = CloseColor;
    }
    protected override void OnPointer(PointerEventData data)
    {
        if (IsOpened) return;
        image.color += Sub;
    }
    protected override void OutPointer(PointerEventData data)
    {
        if (IsOpened) return;
        image.color = BfColor;
    }
    protected override void Click(PointerEventData Data)
    {
        if (IsOpened) return;
        OpenTab();
    }

    public void OpenTab()
    {
        foreach (GameObject s in Subs) s.SetActive(true);
        image.color = OpenColor;
        GOF.ChangeTab(MyIndex);
        IsOpened = true;
    }

    public void CloseTab()
    {
        IsOpened = false;
        try { image.color = CloseColor; } catch{ }
        foreach (GameObject s in Subs) s.SetActive(false);
    }
}
