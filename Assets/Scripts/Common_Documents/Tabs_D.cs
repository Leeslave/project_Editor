using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tabs_D : Buttons_M
{
    public List<GameObject> Subs;
    [SerializeField] int MyIndex;
    public bool IsOpened;

    Color Sub = new Color(0.1f, 0.1f, 0.1f);
    [SerializeField] Sprite NonSelect;
    [SerializeField] Sprite Select;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnPointer(PointerEventData data)
    {
        if (IsOpened) return;
        image.sprite = Select;
    }
    protected override void OutPointer(PointerEventData data)
    {
        if (IsOpened) return;
        image.sprite = NonSelect;
    }
    protected override void Click(PointerEventData Data)
    {
        if (IsOpened) return;
        OpenTab();
    }

    public void OpenTab()
    {
        foreach (GameObject s in Subs) s.SetActive(true);
        image.sprite = Select;
        DB_M.DB_Docs.GetOption.ChangeTab(MyIndex);
        IsOpened = true;
    }

    public void CloseTab()
    {
        IsOpened = false;
        try { image.sprite = NonSelect; } catch{ }
        foreach (GameObject s in Subs) s.SetActive(false);
    }
}
